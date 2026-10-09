import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { UiInput } from '../../../shared/components/ui-input/ui-input';
import { UiButton } from '../../../shared/components/ui-button/ui-button';

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, UiInput, UiButton],
  templateUrl: './settings.html',
  styleUrl: './settings.scss',
})
export class Settings implements OnInit {
  settingsForm: FormGroup;
  isSaved = false;

  constructor(private fb: FormBuilder) {
    this.settingsForm = this.fb.group({
      fullName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      phone: [''],
      companyName: ['']
    });
  }

  ngOnInit(): void {
    // Dummy data load
    this.settingsForm.patchValue({
      fullName: 'Admin User',
      email: 'admin@evenex.com',
      phone: '+90 555 123 4567',
      companyName: 'Evenex Corp'
    });
  }

  onSubmit() {
    if (this.settingsForm.valid) {
      console.log('Settings saved', this.settingsForm.value);
      this.isSaved = true;
      setTimeout(() => this.isSaved = false, 3000);
    }
  }
}
