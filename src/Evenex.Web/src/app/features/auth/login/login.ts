import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { UiButton } from '../../../shared/components/ui-button/ui-button';
import { UiInput } from '../../../shared/components/ui-input/ui-input';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [RouterLink, UiButton, UiInput],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class Login {}
