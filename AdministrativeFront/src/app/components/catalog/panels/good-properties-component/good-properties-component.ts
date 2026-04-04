import { Component, signal } from '@angular/core';
import { Button } from "../../../controls/button/button";
import { GoodPropertiesPopup } from '../good-properties-popup/good-properties-popup';

@Component({
  selector: 'app-good-properties-component',
  imports: [Button, GoodPropertiesPopup],
  templateUrl: './good-properties-component.html',
  styleUrl: './good-properties-component.css',
})
export class GoodPropertiesComponent {
  isPopupShowed = signal<boolean>(false);

  showPopup(): void {
    this.isPopupShowed.set(true);
  }

  closePopup(): void {
    this.isPopupShowed.set(false);
  }
}
