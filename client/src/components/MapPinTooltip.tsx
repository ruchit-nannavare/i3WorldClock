import { useMemo } from 'react';
import { WiDaySunny, WiNightClear } from 'react-icons/wi';
import type { City } from '../types';
import { getLocalTime, formatTimeShort, isDayTime } from '../utils/timeCalculations';
import './MapPinTooltip.css';

interface MapPinTooltipProps {
  city: City;
  utcTime: Date;
  is24Hour: boolean;
  isDarkMode: boolean;
  position: { x: number; y: number };
}

export function MapPinTooltip({
  city,
  utcTime,
  is24Hour,
  isDarkMode,
  position,
}: MapPinTooltipProps) {
  const localTime = useMemo(() => {
    return getLocalTime(utcTime, city.utcOffsetSeconds);
  }, [utcTime, city.utcOffsetSeconds]);

  const timeString = useMemo(() => {
    return formatTimeShort(localTime, is24Hour);
  }, [localTime, is24Hour]);

  const isDay = useMemo(() => {
    return isDayTime(localTime.getUTCHours());
  }, [localTime]);

  const dayNightIcon = isDay ? <WiDaySunny size={18} /> : <WiNightClear size={18} />;
  const dayNightText = isDay ? 'Day' : 'Night';

  return (
    <div
      className={`map-pin-tooltip ${isDarkMode ? 'dark' : 'light'}`}
      style={{
        left: `${position.x}px`,
        top: `${position.y}px`,
      }}
    >
      <div className="map-pin-tooltip-header">
        <span className="map-pin-tooltip-city">{city.name}</span>
        <span className="map-pin-tooltip-country">{city.country}</span>
      </div>
      <div className="map-pin-tooltip-info">
        <span className="map-pin-tooltip-time">{timeString}</span>
        <span className="map-pin-tooltip-daytime">
          {dayNightIcon}
          <span>{dayNightText}</span>
        </span>
      </div>
    </div>
  );
}
