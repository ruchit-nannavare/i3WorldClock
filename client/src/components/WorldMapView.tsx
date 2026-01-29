import { useState, memo, useCallback } from 'react';
import { ComposableMap, Geographies, Geography } from 'react-simple-maps';
import type { City } from '../types';
import { MapPin } from './MapPin';
import { MapPinTooltip } from './MapPinTooltip';
import './WorldMapView.css';

const geoUrl = '/maps/world-110m.json';

interface WorldMapViewProps {
  cities: City[];
  primaryCityId: string | null;
  utcTime: Date;
  is24Hour: boolean;
  isDarkMode: boolean;
  isExpanded: boolean;
  onCitySelect: (cityId: string) => void;
}

function WorldMapViewComponent({
  cities,
  primaryCityId,
  utcTime,
  is24Hour,
  isDarkMode,
  isExpanded,
  onCitySelect,
}: WorldMapViewProps) {
  const [hoveredCity, setHoveredCity] = useState<City | null>(null);
  const [tooltipPosition, setTooltipPosition] = useState({ x: 0, y: 0 });

  const handlePinHover = useCallback((city: City | null, clientX?: number, clientY?: number) => {
    setHoveredCity(city);
    if (city && clientX !== undefined && clientY !== undefined) {
      setTooltipPosition({ x: clientX, y: clientY });
    }
  }, []);

  return (
    <div className={`world-map-view ${isExpanded ? 'expanded' : 'collapsed'}`}>
      <div className="world-map-container">
        <ComposableMap
          projection="geoMercator"
          projectionConfig={{
            scale: 147,
          }}
        >
          <Geographies geography={geoUrl}>
            {({ geographies }: { geographies: any[] }) =>
              geographies.map((geo: any) => (
                <Geography
                  key={geo.rsmKey}
                  geography={geo}
                  className={`world-map-geography ${isDarkMode ? 'dark' : 'light'}`}
                />
              ))
            }
          </Geographies>

          {/* Render city pins */}
          {cities.map((city) => (
            <MapPin
              key={city.id}
              city={city}
              isPrimary={city.id === primaryCityId}
              isDarkMode={isDarkMode}
              onClick={onCitySelect}
              onHover={handlePinHover}
              size={isExpanded ? 'large' : 'small'}
            />
          ))}
        </ComposableMap>
      </div>

      {/* Tooltip - only show when expanded */}
      {isExpanded && hoveredCity && (
        <MapPinTooltip
          city={hoveredCity}
          utcTime={utcTime}
          is24Hour={is24Hour}
          isDarkMode={isDarkMode}
          position={tooltipPosition}
        />
      )}
    </div>
  );
}

export const WorldMapView = memo(WorldMapViewComponent);
