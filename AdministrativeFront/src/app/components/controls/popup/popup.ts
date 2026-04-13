import { Component, Input, OnInit, signal } from '@angular/core';
import { Button } from '../button/button';
import { FormsModule } from '@angular/forms';
import { Lookup, LookupData } from '../lookup/lookup';
import { environment } from '../../../../environments/environment';
import { ShopService } from '../../../services/shop-service';

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
  isVisible?: (controlValues: { [id: string]: any }) => boolean;
}

export enum ControlType {
  Input,
  Textarea,
  Lookup,
  Boolean,
  File,
  List
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

  constructor(private shopService: ShopService) {
  }

  ngOnInit(): void {
    console.log(this.config);
    this.config.controls.forEach((control) => {
      if (control.type === ControlType.File) {
        this.controlValues[control.tag] = {
          previewUrl: signal<string>(this.getInitFileSrc(control.value))
        }
      } else if (control.type === ControlType.List) {
        this.controlValues[control.tag] = control.value ? [...control.value] : [];
      } else if (control.type === ControlType.Boolean) {
        this.controlValues[control.tag] = control.value === true || control.value === 'true';
      } else {
        this.controlValues[control.tag] = control.value;
      }
    }, this);
  }

  isControlVisible(control: ControlConfig): boolean {
    if (control.isVisible) {
      return control.isVisible(this.controlValues);
    }
    return true;
  }

  addListItem(tag: string): void {
    const list = this.controlValues[tag] as string[];
    list.push('');
    this.controlValues[tag] = [...list];
  }

  removeListItem(tag: string, index: number): void {
    const list = this.controlValues[tag] as string[];
    list.splice(index, 1);
    this.controlValues[tag] = [...list];
  }

  onListValueChanged(tag: string, index: number, value: string): void {
    const list = this.controlValues[tag] as string[];
    list[index] = value;
  }

  getInitFileSrc(imageId: string): string {
    if (imageId && imageId.length > 0) {
      return `${environment.imageUrl}/${this.shopService.currentShop?.id}/${imageId}`;
    }
    return '';
  }

  onButtonClick(tag: string): void {
    if (this.config.callback instanceof Function) {
      this.config.callback(tag, this.controlValues);
    }
  }

  onLookupSelected(data: LookupData | undefined, tag: string) {
    this.controlValues[tag] = data;
  }

  onFileChanged(event: any, tag: string) {
    const file: File = event.target.files[0];
    if (file) {
      this.controlValues[tag] = {
        file: file,
        previewUrl: signal<string>('')
      };
      const reader = new FileReader();
      reader.onload = (e: any) => this.controlValues[tag].previewUrl.set(e.target.result);
      reader.readAsDataURL(file);
    }
  }

  getFileSrc(tag: string): string {
    return this.controlValues[tag].previewUrl();
    return "placeholder.svg";
  }
}
