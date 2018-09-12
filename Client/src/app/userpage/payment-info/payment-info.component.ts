import { Component, OnInit } from '@angular/core';

import { UserService } from '../../services/user.service';
import { Payment } from '../../Models/payment';

@Component({
  selector: 'app-payment-info',
  templateUrl: './payment-info.component.html',
  styleUrls: ['./payment-info.component.css']
})
export class PaymentInfoComponent implements OnInit {

  payment: Payment;

  constructor(
    private userService: UserService
  ) { }

  ngOnInit() {
    this.getPayment();
  }

  getPayment() {
    console.log('x');
    this.userService.getPayment()
      .subscribe(x => {this.payment = x});
  }

}
