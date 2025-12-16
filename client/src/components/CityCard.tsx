import { useMemo } from 'react';
import { FiX } from 'react-icons/fi';
import { WiDaySunny, WiNightClear } from 'react-icons/wi';
import type { City } from '../types';
import { getLocalTime, formatTimeShort, isDayTime } from '../utils/timeCalculations';
import './CityCard.css';

interface CityCardProps {
  city: City;
  utcTime: Date;
  is24Hour: boolean;
  isPrimary: boolean;
  onClick: () => void;
  onRemove: () => void;
}

export function CityCard({
  city,
  utcTime,
  is24Hour,
  isPrimary,
  onClick,
  onRemove,
}: CityCardProps) {
  const localTime = useMemo(() => {
    return getLocalTime(utcTime, city.utcOffsetSeconds);
  }, [utcTime, city.utcOffsetSeconds]);

  const timeString = useMemo(() => {
    return formatTimeShort(localTime, is24Hour);
  }, [localTime, is24Hour]);

  const isDay = useMemo(() => {
    return isDayTime(localTime.getUTCHours());
  }, [localTime]);

  const dayNightStatus = isDay ? 'Day' : 'Night';
  const dayNightIcon = isDay ? <WiDaySunny size={20} /> : <WiNightClear size={20} />;

  return (
    <div
      className={`city-card ${isPrimary ? 'city-card-selected' : 'city-card-default'}`}
      onClick={onClick}
    >
      <button
        className="city-card-remove"
        onClick={(e) => {
          e.stopPropagation();
          onRemove();
        }}
        aria-label={`Remove ${city.name}`}
      >
        <FiX size={12} />
      </button>
      <div className="city-card-header">
        <span className="city-card-offset">{city.utcOffsetDisplay}</span>
      </div>
      <span className="city-card-name">{city.name}</span>
      <div className="city-card-time-row">
        <span className="city-card-time">{timeString}</span>
        <span className="city-card-day-night">
          {dayNightIcon}
          <span className="city-card-day-night-text">{dayNightStatus}</span>
        </span>
      </div>
    </div>
  );
}
