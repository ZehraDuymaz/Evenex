import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-ui-button',
  templateUrl: './ui-button.html',
  styleUrl: './ui-button.scss'
})
export class UiButton {
  @Input() variant: 'primary' | 'icon' | 'default' = 'default';
  @Input() type: 'button' | 'submit' | 'reset' = 'button';
  @Output() btnClick = new EventEmitter<MouseEvent>();

  getClasses(): string {
    if (this.variant === 'primary') return 'btn btn-primary';
    if (this.variant === 'icon') return 'btn-icon';
    return 'btn btn-default';
  }
}
