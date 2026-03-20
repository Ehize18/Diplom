import { Component, input } from '@angular/core';

@Component({
  selector: 'app-button',
  imports: [],
  templateUrl: './button.html',
  styleUrl: './button.css',
})
export class Button {
  caption = input<string>();
  onClickCallback = input<Function>(function() {});

  onButtonClick(): void {
    if (this.onClickCallback() instanceof Function) {
      this.onClickCallback()();
    }
  }
}
