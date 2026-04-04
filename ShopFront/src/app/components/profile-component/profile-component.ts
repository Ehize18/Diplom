import { Component, OnInit, signal } from '@angular/core';

@Component({
  selector: 'app-profile-component',
  imports: [],
  templateUrl: './profile-component.html',
  styleUrl: './profile-component.css',
})
export class ProfileComponent implements OnInit {
  profileImageSrc = signal<string>('placeholder.svg');
  name = signal<string>('');

  ngOnInit(): void {
    const authType = window.sessionStorage.getItem('AuthType');
    switch (authType) {
      case 'vk':
        this.loadVkData();
        return;
    }
  }

  private loadVkData(): void {
    const vkImagesString = window.sessionStorage.getItem('VkImages');
    if (vkImagesString) {
      let images = JSON.parse(vkImagesString);
      if (images && images.photo_200) {
        this.profileImageSrc.set(images.photo_200);
      }
      else if (images && images.photo_100) {
        this.profileImageSrc.set(images.photo_100);
      }
    }
    const firstName = window.sessionStorage.getItem('VkFirstName');
    const lastName = window.sessionStorage.getItem('VkLastName');
    if (lastName && lastName.length > 0) {
      this.name.set(`${firstName} ${lastName}`);
    }
    else {
      this.name.set(`${firstName}`);
    }
  }
}
