import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { IBrand } from '../shared/models/brand';
import { IProduct } from '../shared/models/product';
import { IType } from '../shared/models/productType';
import { ShopService } from './shop.service';
import { ShopParams } from '../shared/models/shopParams';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-shop',
  templateUrl: './shop.component.html',
  styleUrls: ['./shop.component.scss'],
})
export class ShopComponent implements OnInit {
  @ViewChild('search', { static: false }) searchTerm: ElementRef;
  products: IProduct[];
  productsSub: Subscription;
  brands: IBrand[];
  types: IType[];
  shopParams = new ShopParams();
  totalCount: number;
  sortOptions = [
    { name: 'Alphabetical', value: 'name' },
    { name: 'Price: Low to High', value: 'priceAsc' },
    { name: 'Price: High to Low', value: 'priceDesc' },
  ];

  constructor(private shopService: ShopService) {
    this.shopParams = this.shopService.getShopParams();
  }

  ngOnInit(): void {
    // Initialize the data
    this.getProducts();
    this.getBrands();
    this.getTypes();
  }

  getProducts(useCache = false) {
    this.productsSub = this.shopService.getProducts(useCache).subscribe(response => {
        this.products = response.data;
        this.totalCount = response.count;
      },
      (error) => {
        console.log(error);
      }
    );
  }

  getBrands() {
    this.shopService.getBrands().subscribe(
      (response) => {
        this.brands = [{ id: 0, name: 'All' }, ...response];
      },
      (error) => {
        console.log(error);
      }
    );
  }

  getTypes() {
    this.shopService.getTypes().subscribe(
      (response) => {
        this.types = [{ id: 0, name: 'All' }, ...response];
      },
      (error) => {
        console.log(error);
      }
    );
  }

  onBrandSelected(brandId: number) {
    const shopParams = this.shopService.getShopParams();
    shopParams.brandId = brandId;
    shopParams.pageNumber = 1;
    this.shopService.setShopParams(shopParams);
    this.getProducts();
  }

  onTypeSelected(typeId: number) {
    const shopParams = this.shopService.getShopParams();
    shopParams.typeId = typeId;
    shopParams.pageNumber = 1;
    this.shopService.setShopParams(shopParams);
    this.getProducts();
  }

  onSortSelected(sort: string) {
    const shopParams = this.shopService.getShopParams();
    shopParams.sort = sort;
    this.shopService.setShopParams(shopParams);
    this.getProducts();
  }

  onPageChanged(event: any) {
    const shopParams = this.shopService.getShopParams();
    if (shopParams.pageNumber !== event) {
      shopParams.pageNumber = event;
      this.shopService.setShopParams(shopParams);
      this.getProducts();
    }
  }

  // Search
  onSearch() {
    const shopParams = this.shopService.getShopParams();
    shopParams.search = this.searchTerm.nativeElement.value;
    shopParams.pageNumber = 1;
    this.shopService.setShopParams(shopParams);
    this.getProducts();
  }

  onReset() {
    this.searchTerm.nativeElement.value = '';
    const shopParams = new ShopParams();
    this.shopService.setShopParams(shopParams);
    this.getProducts();
  }
}
