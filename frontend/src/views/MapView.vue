<template>
  <div class="map-container">
    <h1>Bike Rental Map</h1>
    <div id="map" class="map-view"></div>

    <div class="controls">
      <button @click="addBikeMarker">Add Bike Marker</button>
      <button @click="addZone">Add Custom Zone</button>
      <button @click="clearOverlays">Clear Overlays</button>
    </div>
    <div>
      <input
        type="text"
        v-model="authToken"
        placeholder="Enter AUTH token"
        @keyup.enter="setAuthToken"
      />
    </div>
  </div>
</template>

<script>
import L from 'leaflet'
import 'leaflet/dist/leaflet.css'
import { api } from '@/services/api-service'

export default {
  name: 'MapView',
  data() {
    return {
      map: null,
      mapCenter: [54.6775530439872, 25.27774382871562], // Default coordinates (Vilnius)
      overlays: [],
      bikeIcon: null,
      // Zone style options
      zoneStyle: {
        color: '#3388ff',
        weight: 2,
        opacity: 0.7,
        fillOpacity: 0.2,
        fillColor: '#3388ff',
      },
      authToken: localStorage.getItem('authToken') || '',
      zones: [],
      isLoading: false,
      error: null,
    }
  },
  mounted() {
    this.initMap()
    this.createBikeIcon()

    if (this.authToken) {
      console.log('Auth token loaded from storage')
      this.fetchAndDisplayZones()
    }
  },
  methods: {
    initMap() {
      this.map = L.map('map').setView(this.mapCenter, 13)

      L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution:
          '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors',
        maxZoom: 19,
      }).addTo(this.map)

      this.map.on('click', (event) => {
        console.log('Map clicked at:', event.latlng)
      })
    },

    createBikeIcon() {
      this.bikeIcon = L.divIcon({
        html: `<div class="bike-icon">
                <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor" width="24" height="24">
                  <path d="M18.18 10l-1.7-4.68A2.008 2.008 0 0 0 14.6 4H12v2h2.6l1.46 4h-4.81l-.36-1H12V7H7v2h1.75l1.82 5H9.9c-.44-2.23-2.31-3.88-4.65-3.99C2.45 9.87 0 12.2 0 15c0 2.8 2.2 5 5 5 2.46 0 4.45-1.69 4.9-4h4.2c.44 2.23 2.31 3.88 4.65 3.99 2.8.13 5.25-2.19 5.25-5 0-2.8-2.2-5-5-5h-.82zM7.82 16c-.4 1.17-1.49 2-2.82 2-1.68 0-3-1.32-3-3s1.32-3 3-3c1.33 0 2.42.83 2.82 2H5v2h2.82zm6.28-2h-1.4l-.73-2H15c-.44.58-.76 1.25-.9 2zm4.9 4c-1.68 0-3-1.32-3-3 0-.93.41-1.73 1.05-2.28l.96 2.64 1.88-.68-.97-2.67c.03 0 .06-.01.08-.01 1.68 0 3 1.32 3 3s-1.32 3-3 3z"/>
                </svg>
              </div>`,
        className: 'bike-marker-icon',
        iconSize: [30, 30],
        iconAnchor: [15, 15],
        popupAnchor: [0, -15],
      })
    },

    addBikeMarker() {
      const lat = this.mapCenter[0] + (Math.random() - 0.5) * 0.05
      const lng = this.mapCenter[1] + (Math.random() - 0.5) * 0.05

      const marker = L.marker([lat, lng], { icon: this.bikeIcon }).addTo(this.map)

      const popupContent = L.DomUtil.create('div', 'custom-popup')
      popupContent.innerHTML = `
        <h3>Bike #${Math.floor(Math.random() * 1000)}</h3>
        <p>Status: Available</p>
        <p>Battery: 85%</p>
        <button class="popup-button rent-button">Rent Bike</button>
        <button class="popup-button info-button">More Info</button>
      `

      const popup = L.popup().setContent(popupContent)
      marker.bindPopup(popup)

      marker.on('popupopen', () => {
        const rentButton = document.querySelector('.rent-button')
        const infoButton = document.querySelector('.info-button')

        if (rentButton) {
          rentButton.addEventListener('click', () => {
            alert('Bike rental initiated!')
            popup.close()
          })
        }

        if (infoButton) {
          infoButton.addEventListener('click', () => {
            alert('More information about this bike...')
          })
        }
      })

      marker.openPopup()
      this.overlays.push(marker)
    },

    addZone() {
      // Let user draw a polygon zone
      alert('Click on the map to start drawing a zone. Double-click to finish.')

      const points = []
      let polygon = null
      let tempLine = null

      // Create a function to handle map clicks during zone creation
      const handleMapClick = (e) => {
        points.push([e.latlng.lat, e.latlng.lng])

        // Display points being created
        if (points.length === 1) {
          // First point
          tempLine = L.polyline(points, this.zoneStyle).addTo(this.map)
        } else {
          // Update the temporary line
          tempLine.setLatLngs(points)

          // Check for double click to end drawing
          if (e.originalEvent.detail === 2 && points.length >= 3) {
            finishZoneDrawing()
          }
        }
      }

      // Function to finish drawing the zone
      const finishZoneDrawing = () => {
        this.map.off('click', handleMapClick)

        if (tempLine) {
          this.map.removeLayer(tempLine)
        }

        if (points.length >= 3) {
          // Create polygon and add to map
          polygon = L.polygon(points, this.zoneStyle).addTo(this.map)

          // Add popup for zone configuration
          const center = polygon.getBounds().getCenter()
          const zonePopup = L.popup()
            .setLatLng(center)
            .setContent(
              `
              <div class="zone-popup">
                <h3>Custom Zone</h3>
                <div class="color-selection">
                  <label>Zone color:</label>
                  <div class="color-option" style="background-color: #3388ff;" data-color="#3388ff"></div>
                  <div class="color-option" style="background-color: #ff3333;" data-color="#ff3333"></div>
                  <div class="color-option" style="background-color: #33cc33;" data-color="#33cc33"></div>
                </div>
                <label>Zone name:</label>
                <input type="text" class="zone-name" value="Restriction Zone">
                <button class="save-zone-button">Save Zone</button>
              </div>
            `,
            )
            .openOn(this.map)

          // Add event listeners after popup is opened
          polygon.on('popupopen', () => {
            // Color selection
            document.querySelectorAll('.color-option').forEach((option) => {
              option.addEventListener('click', (e) => {
                const color = e.target.getAttribute('data-color')
                polygon.setStyle({
                  color: color,
                  fillColor: color,
                })
              })
            })

            // Save button
            const saveButton = document.querySelector('.save-zone-button')
            if (saveButton) {
              saveButton.addEventListener('click', () => {
                const name = document.querySelector('.zone-name').value
                polygon.bindTooltip(name, {
                  permanent: true,
                  direction: 'center',
                  className: 'zone-label',
                })
                zonePopup.close()
              })
            }
          })

          this.overlays.push(polygon)
        }
      }

      // Add the click handler to the map
      this.map.on('click', handleMapClick)
    },

    clearOverlays() {
      // Remove all overlays from the map
      this.overlays.forEach((overlay) => {
        this.map.removeLayer(overlay)
      })
      this.overlays = []
    },

    setAuthToken() {
      if (this.authToken) {
        api.setAuthToken(this.authToken)
        console.log('Auth token saved:', this.authToken)
        // Fetch zones after setting token
        this.fetchAndDisplayZones()
      }
    },

    async fetchAndDisplayZones() {
      try {
        this.isLoading = true
        this.error = null

        const response = await api.getZones()
        this.zones = response.data

        // Display zones on the map
        this.displayZones()
      } catch (error) {
        console.error('Error fetching zones:', error)
        this.error = 'Failed to fetch zones. Please try again.'
      } finally {
        this.isLoading = false
      }
    },

    displayZones() {
      if (!this.zones || !this.zones.length) return

      this.zones.forEach((zone) => {
        // Create the rectangle for the zone using the coordinates
        const bounds = [
          [zone.latitude1, zone.longitude1], // top-left corner
          [zone.latitude2, zone.longitude2], // bottom-right corner
        ]

        const rectangle = L.rectangle(bounds, {
          color: '#3388ff',
          weight: 2,
          opacity: 0.7,
          fillOpacity: 0.2,
          fillColor: '#3388ff',
        }).addTo(this.map)

        // Add zone name as tooltip
        rectangle.bindTooltip(zone.name, {
          permanent: true,
          direction: 'center',
          className: 'zone-label',
        })

        // Store zone rectangle for later reference
        this.overlays.push(rectangle)

        // Add bike markers if there are bikes in the zone
        if (zone.bikes && zone.bikes.length > 0) {
          this.displayBikesInZone(zone)
        }
      })

      // Adjust map view to fit all zones
      if (this.zones.length > 0) {
        const allBounds = []
        this.zones.forEach((zone) => {
          allBounds.push([zone.latitude1, zone.longitude1])
          allBounds.push([zone.latitude2, zone.longitude2])
        })

        if (allBounds.length > 0) {
          this.map.fitBounds(allBounds)
        }
      }
    },

    displayBikesInZone(zone) {
      if (!zone.bikes || !zone.bikes.length) return

      zone.bikes.forEach((bike) => {
        // Calculate a random position within the zone rectangle for each bike
        const lat = zone.latitude1 + (zone.latitude2 - zone.latitude1) * Math.random()
        const lng = zone.longitude1 + (zone.longitude2 - zone.longitude1) * Math.random()

        const marker = L.marker([lat, lng], { icon: this.bikeIcon }).addTo(this.map)

        const popupContent = L.DomUtil.create('div', 'custom-popup')
        popupContent.innerHTML = `
          <h3>Bike #${bike.id.substring(0, 8)}</h3>
          <p>Model: ${bike.model || 'Standard'}</p>
          <p>Status: ${bike.status || 'Available'}</p>
          <p>Price: ${bike.rentPrice}€/min</p>
          <button class="popup-button rent-button" data-bike-id="${bike.id}">Rent Bike</button>
          <button class="popup-button info-button" data-bike-id="${bike.id}">More Info</button>
        `

        const popup = L.popup().setContent(popupContent)
        marker.bindPopup(popup)

        marker.on('popupopen', () => {
          const rentButton = document.querySelector('.rent-button')
          const infoButton = document.querySelector('.info-button')

          if (rentButton) {
            rentButton.addEventListener('click', () => {
              this.rentBike(bike.id)
              popup.close()
            })
          }

          if (infoButton) {
            infoButton.addEventListener('click', () => {
              this.showBikeInfo(bike)
            })
          }
        })

        this.overlays.push(marker)
      })
    },

    rentBike(bikeId) {
      alert(`Starting rental process for bike: ${bikeId}`)
    },

    showBikeInfo(bike) {
      alert(`Bike Details:
Model: ${bike.model}
Status: ${bike.status}
Lock Status: ${bike.lockStatus}
ID: ${bike.id}`)
    },
  },
}
</script>

<style scoped>
.map-container {
  display: flex;
  flex-direction: column;
  height: 100%;
}

.map-view {
  width: 100%;
  height: 500px;
  margin-bottom: 20px;
}

.controls {
  display: flex;
  gap: 10px;
  margin-bottom: 20px;
}

button {
  padding: 8px 12px;
  background-color: #4caf50;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

button:hover {
  background-color: #45a049;
}

/* Custom bike marker styling */
:deep(.bike-marker-icon) {
  background: none;
  border: none;
}

:deep(.bike-icon) {
  color: #ff5722;
  background-color: white;
  border-radius: 50%;
  padding: 3px;
  box-shadow: 0 0 5px rgba(0, 0, 0, 0.4);
}

/* Custom popup styling */
:deep(.custom-popup) {
  min-width: 200px;
}

:deep(.popup-button) {
  margin-top: 5px;
  margin-right: 5px;
  padding: 4px 8px;
  border-radius: 4px;
  border: none;
  cursor: pointer;
}

:deep(.rent-button) {
  background-color: #4caf50;
  color: white;
}

:deep(.info-button) {
  background-color: #2196f3;
  color: white;
}

/* Zone styling */
:deep(.zone-popup) {
  min-width: 200px;
}

:deep(.color-selection) {
  display: flex;
  align-items: center;
  gap: 8px;
  margin: 10px 0;
}

:deep(.color-option) {
  width: 20px;
  height: 20px;
  border-radius: 50%;
  cursor: pointer;
  border: 1px solid #ccc;
}

:deep(.zone-name) {
  width: 100%;
  padding: 5px;
  margin: 5px 0 10px 0;
}

:deep(.save-zone-button) {
  background-color: #2196f3;
  color: white;
  padding: 5px 10px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

:deep(.zone-label) {
  background: rgba(255, 255, 255, 0.8);
  border: none;
  border-radius: 4px;
  padding: 5px;
  font-weight: bold;
}
</style>
