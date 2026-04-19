import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';

export interface ColorSetting {
  Variable: string;
  Label: string;
  Value: string;
}

@Injectable({
  providedIn: 'root',
})
export class ColorService {
  private httpClient: HttpClient;

  constructor(httpClient: HttpClient) {
    this.httpClient = httpClient;
  }

  async loadAndApplyColors(shopId: string): Promise<boolean> {
    let result = false;
    await this.httpClient.get<ColorSetting[]>(environment.IMAGES_URL + `/${shopId}/colors.json`).forEach(
      (colors) => {
        if (colors && colors.length > 0) {
          colors.forEach((c) => {
            document.body.style.setProperty(c.Variable, c.Value);
          });
          result = true;
        }
      }
    )
    .catch(() => {
      result = false;
    });
    return result;
  }
}
