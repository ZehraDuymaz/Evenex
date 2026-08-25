import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-ui-input',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './ui-input.html',
  styleUrl: './ui-input.scss'
})
export class UiInput {
  @Input() label = '';
  @Input() type = 'text';
  @Input() value = '';
  @Input() readonly = false;
  @Input() mono = false;
  @Input() hasIcon = false;
}
