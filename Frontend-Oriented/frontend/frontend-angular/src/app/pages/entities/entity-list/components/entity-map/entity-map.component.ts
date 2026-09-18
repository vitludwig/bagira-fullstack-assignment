import {
  afterNextRender,
  ChangeDetectionStrategy,
  Component,
  DestroyRef,
  effect,
  ElementRef,
  inject,
  input,
  viewChild,
} from '@angular/core';
import { circleMarker, LatLngBounds, layerGroup, map, Map, tileLayer } from 'leaflet';
import { IEntity } from '../../types/IEntity';

@Component({
  selector: 'app-entity-map',
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './entity-map.component.html',
  styleUrl: './entity-map.component.scss',
})
export class EntityMapComponent {
  private readonly destroyRef = inject(DestroyRef);
  private readonly mapContainer = viewChild.required<ElementRef<HTMLElement>>('mapContainer');
  private readonly markers = layerGroup();
  private mapInstance: Map | null = null;

  readonly entities = input.required<IEntity[]>();

  constructor() {
    afterNextRender(() => this.initializeMap());
    effect(() => this.renderEntities(this.entities()));
    this.destroyRef.onDestroy(() => this.mapInstance?.remove());
  }

  private initializeMap(): void {
    this.mapInstance = map(this.mapContainer().nativeElement, {
      center: [0, 0],
      zoom: 2,
    });

    tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
      maxZoom: 19,
      attribution: '&copy; OpenStreetMap contributors',
    }).addTo(this.mapInstance);
    this.markers.addTo(this.mapInstance);
    this.renderEntities(this.entities());
  }

  private renderEntities(entities: IEntity[]): void {
    this.markers.clearLayers();

    if (!this.mapInstance || entities.length === 0) {
      return;
    }

    const bounds = new LatLngBounds([]);
    for (const entity of entities) {
      const position: [number, number] = [entity.latitude, entity.longitude];
      const popup = document.createElement('div');
      const name = document.createElement('strong');
      const details = document.createElement('div');

      name.textContent = entity.name;
      details.textContent = `${entity.type} · ${entity.taskForce}`;
      popup.append(name, details);

      circleMarker(position, { radius: 8 }).bindPopup(popup).addTo(this.markers);
      bounds.extend(position);
    }

    if (entities.length === 1) {
      this.mapInstance.setView(bounds.getCenter(), 12);
      return;
    }

    this.mapInstance.fitBounds(bounds, { padding: [24, 24] });
  }
}
