import { memo } from 'react';
import { Marker } from 'react-simple-maps';
import type { City } from '../types';
import { isValidCoordinate } from '../utils/mapProjection';
import './MapPin.css';

interface MapPinProps {
  city: City;
  isPrimary: boolean;
  isDarkMode: boolean;
  onClick: (cityId: string) => void;
  onHover?: (city: City | null, clientX?: number, clientY?: number) => void;
  size?: 'small' | 'medium' | 'large';
}

function MapPinComponent({
  city,
  isPrimary,
  isDarkMode,
  onClick,
  onHover,
  size = 'medium',
}: MapPinProps) {
  // Validate coordinates
  if (!isValidCoordinate(city.latitude, city.longitude)) {
    console.warn(`Invalid coordinates for city ${city.name}: [${city.latitude}, ${city.longitude}]`);
    return null;
  }

  const sizeMap = {
    small: isPrimary ? 3 : 2,
    medium: isPrimary ? 7 : 5,
    large: isPrimary ? 10 : 7,
  };

  const radius = sizeMap[size];

  const handleMouseEnter = (e: any) => {
    if (onHover) {
      const event = e.nativeEvent || e;
      onHover(city, event.clientX, event.clientY);
    }
  };

  const handleMouseLeave = () => {
    if (onHover) {
      onHover(null);
    }
  };

  return (
    <Marker
      coordinates={[city.longitude, city.latitude]}
      onMouseEnter={handleMouseEnter}
      onMouseLeave={handleMouseLeave}
      onClick={() => onClick(city.id)}
    >
      <g className={`map-pin ${isPrimary ? 'map-pin-primary' : 'map-pin-secondary'} ${isDarkMode ? 'dark' : 'light'}`} style={{ cursor: 'pointer' }}>
        <circle
          r={radius}
          className="map-pin-circle"
        />
        {isPrimary && size !== 'small' && (
          <circle
            r={radius + 4}
            className="map-pin-pulse"
          />
        )}
      </g>
    </Marker>
  );
}

export const MapPin = memo(MapPinComponent);
