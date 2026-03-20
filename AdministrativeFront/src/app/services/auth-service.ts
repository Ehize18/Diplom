import { HttpClient } from '@angular/common/http';
import { Injectable, OnInit, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { AuthResponse } from '../contracts/auth';
import { User } from '../contracts/user';

@Injectable({
  providedIn: 'root',
})
export class AuthService implements OnInit {
  public get isAuth() {
    return this._isAuth();
  }
  private set isAuth(value: boolean) {
    this._isAuth.set(value);
  }
  private _isAuth = signal(false);

  public get user() {
    const userString = window.localStorage.getItem("CurrentUser");
    if (userString) {
      const user: User = JSON.parse(userString);
      return user;
    }
    return undefined;
  }

  private set user(value) {
    const userString = JSON.stringify(value);
    window.localStorage.setItem("CurrentUser", userString);
  }

  private baseUrl: string;

  constructor(private httpClient: HttpClient) {
    this.baseUrl = environment.apiUrl + "/Auth"
  }

  ngOnInit(): void {
    
  }

  async Login(): Promise<void> {
    if (this.isAuth) {
      return;
    }

    this.httpClient.post<AuthResponse>(
      this.baseUrl,
      {}, {
        withCredentials: true
      }
    ).subscribe({
      next: async (value) => {
        this.user = {
          id: value.id,
          username: value.username
        };

        this.isAuth = true;
      },
      error: (err) => {
        console.log(err);
      }
    });
  }

  async Logout(): Promise<void> {
    if (!this.isAuth) {
      return;
    }
    await cookieStore.delete('Username');
    this.isAuth = false;
  }
}
