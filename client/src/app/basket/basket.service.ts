import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Basket, IBasket, IBasketItem, IBasketTotals } from '../shared/models/basket';
import { IProduct } from '../shared/models/product';
import { IDeliveryMethod } from '../shared/models/deliveryMethod';

@Injectable({
  providedIn: 'root',
})
export class BasketService {
  baseUrl = environment.apiUrl;
  private basketSource = new BehaviorSubject<IBasket>(null);
  private basketTotalSource = new BehaviorSubject<IBasketTotals>(null);
  basket$ = this.basketSource.asObservable();
  basketTotal$ = this.basketTotalSource.asObservable();
  shipping = 0;
  
  constructor(private http: HttpClient) {}

  setShippingPrice(deliveryMethod: IDeliveryMethod) {
    this.shipping = deliveryMethod.price;
    this._calculateTotals();
  }
  
  getBasket(id: string) {
    return this.http.get<IBasket>(`${this.baseUrl}/basket?id=${id}`).pipe(
      map((basket: IBasket) => {
        this.basketSource.next(basket);
        this._calculateTotals();
      })
    );
  }
  
  setBasket(basket: Basket) {
    return this.http.post<IBasket>(`${this.baseUrl}/basket`, basket).subscribe((response: IBasket) => {
      this.basketSource.next(response);
      this._calculateTotals();
    }, error => {
      console.log(error);
    });
  }
  
  getCurrentBasketValue() {
    return this.basketSource.value;
  }
  
  addItemToBasket(item: IProduct, quantity = 1) {
    const itemToAdd: IBasketItem = this._mapProductItemToBasketItem(
      item,
      quantity
    );
    const basket = this.getCurrentBasketValue() ?? this._createBasket();
    basket.items = this._addOrUpdateItem(basket.items, itemToAdd, quantity);
    this.setBasket(basket);
  }
  
  incrementItemQuantity(item: IBasketItem) {
    const basket = this.getCurrentBasketValue();
    const foundItemIndex = basket.items.findIndex(i => i.id === item.id);
    basket.items[foundItemIndex].quantity++;
    this.setBasket(basket);
  }

  decrementItemQuantity(item: IBasketItem) {
    const basket = this.getCurrentBasketValue();
    const foundItemIndex = basket.items.findIndex(i => i.id === item.id);
    if (basket.items[foundItemIndex].quantity > 1) {
      basket.items[foundItemIndex].quantity--;
      this.setBasket(basket);
    } else {
      this.removeItemFromBasket(item);
    }
  }
  
  removeItemFromBasket(item: IBasketItem) {
    const basket = this.getCurrentBasketValue();
    if (basket.items.some(x => x.id === item.id)) {
      basket.items = basket.items.filter(i => i.id !== item.id);
      if (basket.items.length > 0) {
        this.setBasket(basket);
      } else {
        this.deleteBasket(basket);
      }
    }
  }

  deleteBasket(basket: IBasket) {
    return this.http.delete(`${this.baseUrl}/basket?id=${basket.id}`).subscribe(
      () => {
        this.basketSource.next(null);
        this.basketTotalSource.next(null);
        localStorage.removeItem('basket_id');
      },
      error => {
        console.log(error);
      }
    );
  }
  
  private _addOrUpdateItem(
    items: IBasketItem[],
    itemToAdd: IBasketItem,
    quantity: number
  ): IBasketItem[] {
    const index = items.findIndex((i) => i.id === itemToAdd.id);
    if (index === -1) {
      itemToAdd.quantity = quantity;
      items.push(itemToAdd);
    } else {
      items[index].quantity += quantity;
    }

    return items;
  }
  
  private _createBasket(): IBasket {
    const basket = new Basket();
    localStorage.setItem('basket_id', basket.id);
    return basket;
  }
  
  private _mapProductItemToBasketItem(
    item: IProduct,
    quantity: number
  ): IBasketItem {
    return {
      id: item.id,
      productName: item.name,
      price: item.price,
      pictureUrl: item.pictureUrl,
      quantity,
      brand: item.productBrand,
      type: item.productType,
    };
  }
  
  private _calculateTotals() {
    const basket = this.getCurrentBasketValue();
    const shipping = this.shipping;
    const subtotal = basket.items.reduce((a, b) => b.price * b.quantity + a, 0);
    const total = subtotal + shipping;
    this.basketTotalSource.next({ shipping, total, subtotal });
  }
}
