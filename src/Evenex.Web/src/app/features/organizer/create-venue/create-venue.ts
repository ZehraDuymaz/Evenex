import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { UiInput } from '../../../shared/components/ui-input/ui-input';
import { UiButton } from '../../../shared/components/ui-button/ui-button';

@Component({
  selector: 'app-create-venue',
  standalone: true,
  imports: [CommonModule, RouterModule, ReactiveFormsModule, UiInput, UiButton],
  templateUrl: './create-venue.html',
  styleUrl: './create-venue.scss'
})
export class CreateVenue {
  venueForm: FormGroup;

  constructor(private fb: FormBuilder) {
    this.venueForm = this.fb.group({
      name: ['', Validators.required],
      city: ['', Validators.required],
      address: ['', Validators.required],
      capacity: ['', [Validators.required, Validators.min(1)]]
    });
  }

  onSubmit() {
    if (this.venueForm.valid) {
      console.log('Venue created', this.venueForm.value);
    }
  }
}
