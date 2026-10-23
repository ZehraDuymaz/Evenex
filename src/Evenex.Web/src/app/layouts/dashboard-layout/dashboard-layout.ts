import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-dashboard-layout',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './dashboard-layout.html',
  styleUrl: './dashboard-layout.scss'
})
export class DashboardLayout {
  // Sağ üstte giriş yapan kullanıcının e-postasını göstermek ve çıkış yapmak için
  readonly auth = inject(AuthService);

  isSidebarCollapsed = signal(false);
  isDarkMode = signal(false);

  toggleSidebar() {
    this.isSidebarCollapsed.update(v => !v);
  }

  toggleTheme() {
    this.isDarkMode.update(v => !v);
    if (this.isDarkMode()) {
      document.documentElement.classList.add('dark');
    } else {
      document.documentElement.classList.remove('dark');
    }
  }

  logout() {
    this.auth.logout();
  }
}
