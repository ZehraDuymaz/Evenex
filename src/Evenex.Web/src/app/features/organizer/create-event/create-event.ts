import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { UiInput } from '../../../shared/components/ui-input/ui-input';
import { UiButton } from '../../../shared/components/ui-button/ui-button';

@Component({
  selector: 'app-create-event',
  standalone: true,
  imports: [CommonModule, RouterModule, ReactiveFormsModule, UiInput, UiButton],
  templateUrl: './create-event.html',
  styleUrl: './create-event.scss'
})
export class CreateEvent {
  eventForm: FormGroup;

  constructor(private fb: FormBuilder) {
    this.eventForm = this.fb.group({
      date: ['', Validators.required],
      time: ['', Validators.required],
      details: ['', Validators.required],
      price: ['', [Validators.required, Validators.min(0)]],
      ticketSaleStartDate: ['', Validators.required]
    });
  }

  onSubmit() {
    if (this.eventForm.valid) {
      console.log('Event created', this.eventForm.value);
    }
  }
}
