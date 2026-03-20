import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { StatisticPanel, StatisticType } from "../panels/statistic-panel/statistic-panel";
import { MethodsPanel, MethodType } from '../panels/methods-panel/methods-panel';
import { IntegrationPanel, IntegrationType } from '../panels/integration-panel/integration-panel';
import { LastEventsPanel } from '../panels/last-events-panel/last-events-panel';

@Component({
  selector: 'app-shop-component',
  imports: [CommonModule, StatisticPanel, MethodsPanel, IntegrationPanel, LastEventsPanel],
  templateUrl: './shop-component.html',
  styleUrl: './shop-component.css',
})
export class ShopComponent {
  statisticType = StatisticType;
  methodType = MethodType;
  integrationType = IntegrationType;
}
