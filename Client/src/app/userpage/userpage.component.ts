import { Component, OnInit } from '@angular/core';

import { UserService } from '../services/user.service';
import { User } from '../Models/user';
import { Payment } from '../Models/payment';



@Component({
  selector: 'app-userpage',
  templateUrl: './userpage.component.html',
  styleUrls: ['./userpage.component.css']
})
export class UserpageComponent implements OnInit {

 //user: User;
 //payment: Payment;

  constructor(
    //private userService: UserService,
  ) { }

  ngOnInit() {
    //this.getUser();
  }

  /*getUser(): void {
    this.userService.getCurrentUser()
      .subscribe(x => this.user = x);
  }*/
}
