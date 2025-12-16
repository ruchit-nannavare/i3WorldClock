import { 
  WiDaySunny, 
  WiCloudy, 
  WiDayCloudy, 
  WiRain, 
  WiSnow, 
  WiThunderstorm, 
  WiFog 
} from 'react-icons/wi';
import type { Weather, WeatherCondition } from '../types';
import './WeatherDisplay.css';

const getWeatherIcon = (condition: WeatherCondition) => {
  switch (condition) {
    case 'Clear':
      return <WiDaySunny size={24} />;
    case 'PartlyCloudy':
      return <WiDayCloudy size={24} />;
    case 'Cloudy':
      return <WiCloudy size={24} />;
    case 'Rain':
      return <WiRain size={24} />;
    case 'Snow':
      return <WiSnow size={24} />;
    case 'Thunderstorm':
      return <WiThunderstorm size={24} />;
    case 'Fog':
      return <WiFog size={24} />;
    default:
      return <WiDaySunny size={24} />;
  }
};

interface WeatherDisplayProps {
  weather: Weather;
}

export function WeatherDisplay({ weather }: WeatherDisplayProps) {
  return (
    <div className="weather-display">
      <span className="weather-icon">{getWeatherIcon(weather.condition)}</span>
      <span className="weather-temp">{Math.round(weather.temperature)}°C</span>
      <span className="weather-condition">{weather.condition}</span>
    </div>
  );
}
