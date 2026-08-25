import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { UiButton } from '../../../shared/components/ui-button/ui-button';

@Component({
  selector: 'app-events',
  imports: [RouterModule, UiButton],
  templateUrl: './events.html',
  styleUrl: './events.scss',
})
export class Events {}
