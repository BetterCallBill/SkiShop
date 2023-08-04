import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { IOrder } from 'src/app/shared/models/order';

@Component({
  selector: 'app-checkout-success',
  templateUrl: './checkout-success.component.html',
  styleUrls: ['./checkout-success.component.scss']
})
export class CheckoutSuccessComponent implements OnInit {
  order: IOrder;

  constructor(private router: Router) {
    // get navigation 
    const navigation = this.router.getCurrentNavigation();
    /*
    ** from checkout payment
    ** const navigationExtras: NavigationExtras = { state: createdOrder };
    ** this.router.navigate(['checkout/success'], navigationExtras);
    */
    const state = navigation && navigation.extras && navigation.extras.state;
 
    if (state) {
      this.order = state as IOrder;
    }
  }

  ngOnInit(): void {
  }

}
