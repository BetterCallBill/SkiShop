import { Component, OnInit } from '@angular/core';
import { IOrder } from 'src/app/shared/models/order';
import { ActivatedRoute } from '@angular/router';
import { BreadcrumbService } from 'xng-breadcrumb';
import { OrdersService } from '../orders.service';

@Component({
  selector: 'app-order-details',
  templateUrl: './order-details.component.html',
  styleUrls: ['./order-details.component.scss']
})
export class OrderDetailsComponent implements OnInit {
  order: IOrder;

  constructor(
    private route: ActivatedRoute,
    private orderService: OrdersService,
    private breadcrumbService: BreadcrumbService
  ) {
    this.breadcrumbService.set('@OrderDetails', '');
  }

  ngOnInit(): void {
    this.orderService.getOrderDetails(+this.route.snapshot.paramMap.get('id'))
      .subscribe((order: IOrder) => {
        this.order = order;
        this.breadcrumbService.set('@OrderDetails', `Order# ${order.id} - ${order.status}`);
        console.log(`Order# ${order.id} - ${order.status}`)
      }, error => console.log(error));
  }
}
