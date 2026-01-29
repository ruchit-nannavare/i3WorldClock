# Quick Start - Interactive World Map

## Testing the Feature

### 1. Start the Application
```bash
# Frontend (if not already running)
cd client
npm run dev
# Access: http://localhost:5174

# Backend (if not already running)
cd ../src/TimeSpot.Server
dotnet run
# API: http://localhost:5000
```

### 2. Test Basic Functionality
1. Open http://localhost:5174 in your browser
2. You should see a small map in the bottom-right corner (mini-map)
3. Click the expand button (⛶ icon) next to the mini-map
4. Watch the clock shrink to the corner and map expand to full screen
5. Click any glowing pin on the map to select a city
6. Hover over pins to see tooltips
7. Click the minimize button (⛉ icon) or press ESC to return to clock view

### 3. Test City Management
1. Add a new city using the search bar
2. Verify a new pin appears on the map
3. Click the pin to select it
4. Remove a city from the carousel
5. Verify the pin disappears from the map

### 4. Test Theming
1. Toggle dark mode (moon/sun icon in header)
2. Verify map outlines and pin colors change appropriately
3. Expand map in both dark and light modes

---

## Feature Controls

### Toggle Map View
- **Button**: Click the maximize/minimize button (bottom-right)
- **Keyboard**: Press `ESC` to collapse map
- **Mini-Map**: Click the mini-map itself to expand (future enhancement)

### Select City
- **From Map**: Click any pin
- **From Carousel**: Click any city card
- **Result**: Both views update simultaneously

### View City Details
- **Hover**: Move mouse over any pin (when map is expanded)
- **Tooltip Shows**: City name, country, current time, day/night status

---

## Keyboard Shortcuts

| Key | Action |
|-----|--------|
| `ESC` | Collapse map view |
| `Tab` | Navigate between interactive elements |
| `Enter/Space` | Activate focused element |

---

## Visual States

### Default (Clock View)
```
┌────────────────────────────────────┐
│  Header (logo, search, actions)    │
├────────────────────────────────────┤
│                                    │
│         LARGE CLOCK                │
│         12:34:56 PM                │
│                                    │
├────────────────────────────────────┤
│  Footer (weather, location)        │
│  City Carousel (scrollable cards)  │
└────────────────────────────────────┘
                    ┌─────┐
                    │ Map │ ← Mini-map
                    │ [⛶] │ ← Toggle btn
                    └─────┘
```

### Expanded (Map View)
```
┌────────────────────────────────────┐
│                                    │
│      FULL SCREEN WORLD MAP         │
│                                    │
│    ●  ●     ●    ●  ● ← Pins       │
│  ●   ●   ●     ●                   │
│     ●        ●    ●  ●             │
│                                    │
│  ┌────────┐ ← Tooltip on hover     │
│  │ London │                        │
│  │ 18:34  │                        │
│  └────────┘                        │
│                                    │
└────────────────────────────────────┘
 ┌──────┐                    ┌───┐
 │ 6:34 │ ← Mini clock       │[⛉]│ ← Toggle
 └──────┘                    └───┘
```

---

## Troubleshooting

### Map Doesn't Load
- Check browser console for errors
- Verify `/maps/world-110m.json` exists
- Clear browser cache and reload

### Pins Missing
- Verify backend API is running
- Check that cities have valid lat/long coordinates
- Open DevTools → Console for warnings

### Animation Stuttering
- Check system performance (CPU/GPU)
- Try in another browser
- Disable browser extensions

### Tooltip Not Showing
- Ensure map is in expanded view (not mini-map)
- Hover directly over pins (not between them)
- Check z-index conflicts with other elements

---

## Browser Compatibility

| Browser | Minimum Version | Status |
|---------|----------------|--------|
| Chrome | 120+ | ✅ Fully Supported |
| Firefox | 121+ | ✅ Fully Supported |
| Edge | 120+ | ✅ Fully Supported |
| Safari | 17+ | ✅ Fully Supported |
| Opera | 106+ | ✅ Fully Supported |
| IE 11 | N/A | ❌ Not Supported |

---

## Known Issues

### 1. Peer Dependency Warning
**Issue**: npm warns about React version mismatch  
**Impact**: None - feature works perfectly  
**Fix**: Ignore warning or wait for react-simple-maps v4

### 2. Small Screen Tooltips
**Issue**: Tooltips may clip on very small screens  
**Impact**: Minor visual issue  
**Fix**: Tooltip auto-centers to minimize clipping

---

## Performance Tips

### For Best Experience:
- Use hardware-accelerated browsers (Chrome, Edge)
- Ensure GPU acceleration is enabled
- Limit to ~20 cities for optimal performance
- Use latest browser versions

### If Experiencing Lag:
1. Close other browser tabs
2. Disable browser extensions
3. Reduce number of saved cities
4. Try in Incognito/Private mode

---

## Development Notes

### Hot Module Replacement (HMR)
When developing, changes to map components hot-reload instantly:
```bash
# Edit any .tsx or .css file
# Browser updates automatically
```

### Debugging
```javascript
// Enable verbose logging
localStorage.setItem('debug', 'true');

// Check map state
console.log(useMapView());

// Inspect city data
console.log(savedCities);
```

### Custom Styling
Map colors are CSS variables - easily customizable:
```css
/* In App.css or component CSS */
.map-pin.dark.map-pin-primary {
  --pin-color: #your-color;
  --pin-glow: rgba(your-color, 0.7);
}
```

---

## API Integration

### No Additional Endpoints Required!
The map uses existing city data from:
- `GET /api/cities/search?query={q}` - City search
- Cities already include `latitude` and `longitude` fields

### Example City Data:
```json
{
  "id": "city_123",
  "name": "London",
  "country": "United Kingdom",
  "latitude": 51.5074,    // ← Used by map
  "longitude": -0.1278,   // ← Used by map
  "utcOffsetSeconds": 0,
  "utcOffsetDisplay": "UTC+00:00"
}
```

---

## Future Enhancements

Planned for future releases:
- [ ] Zoom and pan controls
- [ ] Draggable mini-map position
- [ ] Pin clustering for dense areas
- [ ] 3D globe view option
- [ ] Time zone visualization overlay
- [ ] Weather overlay on map
- [ ] Animated day/night shadow

---

## Support & Feedback

### Report Issues:
1. Check console for errors
2. Note browser and OS version
3. Describe steps to reproduce
4. Include screenshot if possible

### Documentation:
- Full plan: `docs/MAP_VIEW_IMPLEMENTATION_PLAN.md`
- This guide: `docs/QUICK_START_MAP.md`
- Completion summary: `docs/MAP_VIEW_IMPLEMENTATION_COMPLETE.md`

---

## Success Checklist

Before marking feature complete, verify:
- [x] Mini-map visible in bottom-right corner
- [x] Toggle button responsive
- [x] Clock minimizes to corner on expand
- [x] Map fills entire screen when expanded
- [x] Pins appear at correct locations
- [x] Tooltips show on hover
- [x] Clicking pins selects cities
- [x] ESC key collapses map
- [x] Dark mode works correctly
- [x] Animations are smooth (60fps)
- [x] No console errors
- [x] Build completes successfully

---

**Happy mapping! 🗺️✨**
