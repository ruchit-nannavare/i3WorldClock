import { WiSunrise } from 'react-icons/wi';
import { calculateSunlightDuration } from '../utils/timeCalculations';
import './SunriseSunset.css';

interface SunriseSunsetProps {
  sunrise: string;
  sunset: string;
}

export function SunriseSunset({ sunrise, sunset }: SunriseSunsetProps) {
  const duration = calculateSunlightDuration(sunrise, sunset);

  return (
    <div className="sunrise-sunset">
      <span className="sunrise-sunset-icon">
        <WiSunrise size={18} />
      </span>
      <span className="sunrise-sunset-text">
        {sunrise} - {sunset} ({duration})
      </span>
    </div>
  );
}
