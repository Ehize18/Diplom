import { Component, Input, OnInit, signal } from '@angular/core';
import { Button } from '../button/button';
import { FormsModule } from '@angular/forms';
import { Lookup, LookupData } from '../lookup/lookup';

export interface PopupConfig {
  id: string,
  controls: ControlConfig[]
  buttons: ButtonConfig[]
  callback: Function
}

export interface ControlConfig {
  type: ControlType,
  tag: string,
  caption: string
  value: any;
  lookupData: LookupData[]
}

export enum ControlType {
  Input,
  Textarea,
  Lookup,
  Boolean
}

export interface ButtonConfig {
  caption: string,
  tag: string
}

@Component({
  selector: 'app-popup',
  imports: [Button, Lookup, FormsModule],
  templateUrl: './popup.html',
  styleUrl: './popup.css',
})
export class Popup implements OnInit {
  @Input() config!: PopupConfig;
  controlType = ControlType;
  controlValues: { [id: string] : any } = {};

  ngOnInit(): void {
    console.log(this.config);
    this.config.controls.forEach((control) => {
      this.controlValues[control.tag] = control.value;
    }, this);
  }

  onButtonClick(tag: string): void {
    if (this.config.callback instanceof Function) {
      this.config.callback(tag, this.controlValues);
    }
  }

  onLookupSelected(data: LookupData | undefined, tag: string) {
    this.controlValues[tag] = data;
  }
}
