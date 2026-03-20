import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth-service';

@Component({
  selector: 'app-home-component',
  imports: [],
  templateUrl: './home-component.html',
  styleUrl: './home-component.css',
})
export class HomeComponent {
  private authService: AuthService
  private router: Router

  constructor(authService: AuthService, router: Router) {
    this.authService = authService
    this.router = router
  }

  async onGoToProfileClick(): Promise<void> {
    if (this.authService.isAuth) {
      this.router.navigate(['/shop']);
    }
    else {
      await this.authService.Login();
    }
  }
}
