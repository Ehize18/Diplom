import { Component, effect, HostListener, input, Input, OnInit, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

export interface LookupData {
  id: string
  caption: string
}

@Component({
  selector: 'app-lookup',
  imports: [FormsModule],
  templateUrl: './lookup.html',
  styleUrl: './lookup.css',
})
export class Lookup {
  @Input() caption!: string;
  @Input() id!: string;
  defaultValue = input<LookupData | undefined>();
  initialValue: LookupData | undefined;
  values = input<LookupData[]>([]);
  itemSelected = output<LookupData | undefined>();
  selectedItem: LookupData | undefined;
  selectedItemCaption: string | undefined;

  constructor() {
    effect(() => {
      if (this.initialValue?.id !== this.defaultValue()?.id) {
        this.initialValue = this.defaultValue();
        this.selectedItem = this.defaultValue();
        this.selectedItemCaption = this.defaultValue()?.caption;
      }
    });
  }

  onValueChange(value: string): void {
  }

  isValuesVisible = signal(false);

  onFocus(e: FocusEvent): void {
    if (e.type === 'focus') {
      this.isValuesVisible.set(true);
      return;
    }
  }

  getListStyle(): string {
    if (this.isValuesVisible()){
      return 'block';
    }
    return 'none';
  }

  getListValues(): LookupData[] {
    let result = this.values();
    if (this.selectedItemCaption && this.selectedItemCaption.length > 0) {
      result = result.filter(
        (item) => item.caption.indexOf(this.selectedItemCaption!) > -1, this
      );
    }
    return result;
  }

   onOptionClick(value: LookupData): void {
    this.selectedItemCaption = value.caption;
    this.isValuesVisible.set(false);
    this.setItem();
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: Event): void {
    if (this.isValuesVisible()) {
      const element = document.querySelector(`#${this.id}`);
      if (!element?.contains(event.target as HTMLElement)) {
        this.isValuesVisible.set(false);
        this.setItem();
      }
    }
  }

  setItem(): void {
    const items = this.values();
    const item = items.find((item) => item.caption === this.selectedItemCaption);
    this.selectedItem = item;
    this.selectedItemCaption = item?.caption;
    this.itemSelected.emit(item);
  }
}
