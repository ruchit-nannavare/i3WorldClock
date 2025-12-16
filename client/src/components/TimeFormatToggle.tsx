import './TimeFormatToggle.css';

interface TimeFormatToggleProps {
  is24Hour: boolean;
  onChange: (is24Hour: boolean) => void;
}

export function TimeFormatToggle({ is24Hour, onChange }: TimeFormatToggleProps) {
  return (
    <div className="time-format-toggle">
      <button
        className={`toggle-btn ${!is24Hour ? 'toggle-btn-active' : ''}`}
        onClick={() => onChange(false)}
        aria-label="Switch to 12-hour format"
      >
        12h
      </button>
      <button
        className={`toggle-btn ${is24Hour ? 'toggle-btn-active' : ''}`}
        onClick={() => onChange(true)}
        aria-label="Switch to 24-hour format"
      >
        24h
      </button>
    </div>
  );
}
