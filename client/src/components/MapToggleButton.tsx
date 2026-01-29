import { FiMaximize2, FiMinimize2 } from 'react-icons/fi';
import './MapToggleButton.css';

interface MapToggleButtonProps {
  isExpanded: boolean;
  onClick: () => void;
}

export function MapToggleButton({ isExpanded, onClick }: MapToggleButtonProps) {
  return (
    <button
      className="map-toggle-button"
      onClick={onClick}
      aria-label={isExpanded ? 'Show Clock View' : 'Expand Map View'}
      title={isExpanded ? 'Show Clock' : 'Expand Map'}
    >
      {isExpanded ? <FiMinimize2 size={20} /> : <FiMaximize2 size={20} />}
    </button>
  );
}
