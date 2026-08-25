import { Routes } from '@angular/router';
import { AuthLayout } from './layouts/auth-layout/auth-layout';
import { Login } from './features/auth/login/login';
import { Signup } from './features/auth/signup/signup';
import { DashboardLayout } from './layouts/dashboard-layout/dashboard-layout';
import { DashboardHome } from './features/organizer/dashboard-home/dashboard-home';
import { Venues } from './features/organizer/venues/venues';
import { Events } from './features/organizer/events/events';
import { Settings } from './features/organizer/settings/settings';

import { CreateVenue } from './features/organizer/create-venue/create-venue';
import { CreateEvent } from './features/organizer/create-event/create-event';
import { Home } from './features/attendee/home/home';

export const routes: Routes = [
  { path: '', component: Home, pathMatch: 'full' },
  {
    path: 'auth',
    component: AuthLayout,
    children: [
      { path: 'login', component: Login },
      { path: 'signup', component: Signup },
      { path: '', redirectTo: 'login', pathMatch: 'full' }
    ]
  },
  {
    path: 'admin',
    component: DashboardLayout,
    children: [
      { path: 'dashboard', component: DashboardHome },
      { path: 'venues', component: Venues },
      { path: 'venues/create', component: CreateVenue },
      { path: 'events', component: Events },
      { path: 'events/create', component: CreateEvent },
      { path: 'settings', component: Settings },
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' }
    ]
  },
  { path: '**', redirectTo: '' }
];
