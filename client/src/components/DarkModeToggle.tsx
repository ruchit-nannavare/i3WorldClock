import { FiSun, FiMoon } from 'react-icons/fi';
import './DarkModeToggle.css';

interface DarkModeToggleProps {
  isDark: boolean;
  onChange: (isDark: boolean) => void;
}

export function DarkModeToggle({ isDark, onChange }: DarkModeToggleProps) {
  return (
    <button
      className="dark-mode-toggle"
      onClick={() => onChange(!isDark)}
      aria-label="Toggle dark mode"
    >
      {isDark ? <FiMoon size={18} /> : <FiSun size={18} />}
    </button>
  );
}
