import type { City } from '../types';
import { CityCard } from './CityCard';
import './CityCarousel.css';

interface CityCarouselProps {
  cities: City[];
  utcTime: Date;
  is24Hour: boolean;
  primaryCityId: string | null;
  onSelectCity: (cityId: string) => void;
  onRemoveCity: (cityId: string) => void;
}

export function CityCarousel({
  cities,
  utcTime,
  is24Hour,
  primaryCityId,
  onSelectCity,
  onRemoveCity,
}: CityCarouselProps) {
  return (
    <div className="city-carousel">
      <div className="city-carousel-track">
        {cities.map((city) => (
          <CityCard
            key={city.id}
            city={city}
            utcTime={utcTime}
            is24Hour={is24Hour}
            isPrimary={city.id === primaryCityId}
            onClick={() => onSelectCity(city.id)}
            onRemove={() => onRemoveCity(city.id)}
          />
        ))}
      </div>
    </div>
  );
}
