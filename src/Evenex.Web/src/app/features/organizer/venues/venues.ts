import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { UiButton } from '../../../shared/components/ui-button/ui-button';

@Component({
  selector: 'app-venues',
  imports: [RouterModule, UiButton],
  templateUrl: './venues.html',
  styleUrl: './venues.scss',
})
export class Venues {}
