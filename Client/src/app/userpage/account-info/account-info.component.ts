import { Component, OnInit } from '@angular/core';

import { UserService } from '../../services/user.service';
import { User } from '../../Models/user';

@Component({
  selector: 'app-account-info',
  templateUrl: './account-info.component.html',
  styleUrls: ['./account-info.component.css']
})
export class AccountInfoComponent implements OnInit {

  user: User

  constructor(
    private userService: UserService,
  ) { }

  ngOnInit() {
    this.getUser();
  }

  getUser(): void {
    this.userService.getCurrentUser()
      .subscribe(x => this.user = x);
  }
}
