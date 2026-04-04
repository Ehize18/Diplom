import { Component, Directive, ElementRef, input, OnInit, signal, forwardRef } from '@angular/core';
import { Button } from "../../../controls/button/button";
import { FormsModule } from '@angular/forms';
import { ShopService } from '../../../../services/shop-service';
import { CatalogService } from '../../../../services/catalog-service';
import { Property, PropertyValue } from '../../../../contracts/catalog';

class PropertyVM {
  id: string;
  title: string;
  values = signal<PropertyValueVM[]>([]);
  isChildsLoaded = signal<boolean>(false);
  model: Property | null = null;

  constructor(id: string, title: string) {
    this.id = id;
    this.title = title;
  }

  private setModel(model: Property) {
    this.model = model;
  }

  public static Create(model: Property) {
    const vm = new PropertyVM(model.id, model.title);
    vm.setModel(model);
    const values: PropertyValueVM[] = [];
    model.properties.forEach(value => {
      values.push(PropertyValueVM.Create(value));
    });
    vm.values.set(values);
    return vm;
  }
}

class PropertyValueVM {
  id: string;
  title: string;
  editTitle: string;
  isEdit = signal<boolean>(false);
  isFocusNeeded = signal<boolean>(false);
  model: PropertyValue | null = null;

  constructor(id: string, title: string) {
    this.id = id;
    this.title = title;
    this.editTitle = title;
  }

  private setModel(model: PropertyValue) {
    this.model = model;
  }

  public static Create(model: PropertyValue) {
    const vm = new PropertyValueVM(model.id, model.title);
    vm.setModel(model);
    return vm;
  }

  save(): void {
    this.title = this.editTitle;
    this.isEdit.set(false);
  }

  cancel(): void {
    this.editTitle = this.title;
    this.isEdit.set(false);
  }

  enableEdit(): void {
    this.isEdit.set(true);
    this.isFocusNeeded.set(true);
  }

  onBlur(): void {
    this.save();
  }

  onKeydown(event: KeyboardEvent): void {
    if (event.key === 'Enter') {
      this.save();
    } else if (event.key === 'Escape') {
      this.cancel();
    }
  }
}

@Component({
  selector: 'app-good-properties-popup',
  imports: [Button, FormsModule, forwardRef(() => AutofocusDirective)],
  templateUrl: './good-properties-popup.html',
  styleUrl: './good-properties-popup.css',
})
export class GoodPropertiesPopup implements OnInit {
  callbackFn = input<Function>();
  newPropertyTitle = signal<string>('');
  newPropertyValueTitle = signal<string>('');
  properties = signal<PropertyVM[]>([]);
  selectedProperty = signal<PropertyVM | null>(null);
  selectedPropertyValue = signal<PropertyValueVM | null>(null);

  constructor(
    private shopService: ShopService,
    private catalogService: CatalogService
  ) {
  }

  ngOnInit(): void {
    this.catalogService.getProperties(
      this.shopService.currentShop!.id
    ).subscribe(
      response => {
        console.log(response);
        if (response.isSuccess) {
          const properties: PropertyVM[] = [];
          response.results.forEach(value => {
            properties.push(PropertyVM.Create(value));
          });
          this.properties.set(properties);
        }
      }
    )
  }

  onCloseClick(): void {
    if (this.callbackFn()) {
      this.callbackFn()!();
    }
  }

  onPropertyClick(property: PropertyVM): void {
    if (this.selectedProperty()?.id === property.id) {
      this.selectedProperty.set(null);
    }
    else {
      this.selectedProperty.set(property);
    }
  }

  private timer: any = null

  onValueClick(value: PropertyValueVM): void {
    if (this.selectedPropertyValue()?.id === value.id) {
      this.timer = setTimeout(() => {
        if (!value.isEdit()) {
          this.selectedPropertyValue.set(null);
        }
      }, 300);
    }
    else {
      this.selectedPropertyValue.set(value);
    }
  }

  addNewProperty(): void {
    const title = this.newPropertyTitle();
    if (title.length > 0) {
      this.catalogService.createProperty(
        this.shopService.currentShop!.id,
        title
      ).subscribe(
        id => {
          const property = new PropertyVM(id, title);
          this.properties.update(
            properties => {
              properties.push(property);
              return properties;
            }
          );
          this.onPropertyClick(property);
        }
      )
      this.newPropertyTitle.set('');
    }
  }

  addNewPropertyValue(): void {
    const title = this.newPropertyValueTitle();
    const selectedProperty = this.selectedProperty();
    if (title.length > 0 && selectedProperty) {
      this.catalogService.createPropertyValue(
        this.shopService.currentShop!.id,
        selectedProperty.id,
        title
      ).subscribe(
        id => {
          const value = new PropertyValueVM(id, title);
          selectedProperty.values.update(
            values => {
              if (values.length === 0) {
                return [value];
              }
              values.push(value);
              return values;
            }
          );
          this.onValueClick(value);
        }
      );
      this.newPropertyValueTitle.set('');
    }
  }
}

@Directive({
  selector: '[appAutofocus]'
})
export class AutofocusDirective implements OnInit {
  constructor(private el: ElementRef) {}

  ngOnInit() {
    // Небольшая задержка для гарантии рендера
    setTimeout(() => {
      this.el.nativeElement.focus();
      this.el.nativeElement.select();
    }, 0);
  }
}