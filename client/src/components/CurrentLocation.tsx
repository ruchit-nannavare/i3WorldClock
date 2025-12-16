import './CurrentLocation.css';

interface CurrentLocationProps {
  name: string;
  country: string;
}

export function CurrentLocation({ name, country }: CurrentLocationProps) {
  return (
    <div className="current-location">
      <h2 className="current-location-text">
        {name}, {country}
      </h2>
    </div>
  );
}
