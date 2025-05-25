<template>
  <div class="map-container">
    <h1>Bike Rental Map</h1>
    <div id="map" class="map-view"></div>

    <div class="controls">
      <button @click="addBikeMarker">Add Bike Marker</button>
      <button @click="addZone">Add Custom Zone</button>
      <button @click="clearOverlays">Clear Overlays</button>
    </div>
    <div class="auth-status">
      <span v-if="isLoggedIn">Logged in as: {{ userEmail }}</span>
      <span v-else>Not logged in. Please <router-link to="/login">login</router-link> to access all
        features.</span>
    </div>

    <!-- Zone Bikes Bottom Popup -->
    <div v-if="showZonePopup" class="bottom-popup zone-popup-overlay" :class="{ 'full-screen': isZonePopupExpanded }"
      @click="closeZonePopup">
      <div class="bottom-popup-content" :class="{ 'expanded': isZonePopupExpanded }" @click.stop>
        <div class="popup-header">
          <div class="drag-handle" @click.stop="toggleZonePopupExpanded" @touchstart.stop="handleTouchStart"
            @touchmove.stop="handleTouchMove" @touchend.stop="handleTouchEnd" @mousedown.stop="handleMouseDown">
            <div class="drag-indicator"></div>
          </div>
          <h3>{{ selectedZone?.name }}</h3>
          <button class="close-btn" @click="closeZonePopup">×</button>
        </div>
        <p class="zone-subtitle">{{ selectedZone?.address }}</p>
        <p class="bikes-count">Available bikes: {{ availableBikes.length }}</p>

        <div v-if="availableBikes.length === 0" class="no-bikes">
          <div class="no-bikes-content">
            <p>No bikes available in this zone</p>
          </div>
        </div>

        <div v-else class="bikes-container">
          <div v-for="bike in availableBikes" :key="bike.id" class="bike-card" @click="selectBike(bike)">
            <div class="bike-card-content">
              <h4 class="bike-name">{{ bike.model }}</h4>
              <div class="bike-details">
                <div class="bike-detail">
                  <span>Price per minute:</span>
                  <span class="price">{{ bike.pricePerMinute }}€</span>
                </div>
              </div>
            </div>
            <button class="select-bike-btn">
              <span class="icon">▶</span>
              Select bike
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Reservation Bottom Popup -->
    <div v-if="showReservationPopup" class="bottom-popup reservation-popup-overlay"
      :class="{ 'full-screen': isReservationPopupExpanded }" @click="closeReservationPopup">
      <div class="bottom-popup-content" :class="{ 'expanded': isReservationPopupExpanded }" @click.stop>
        <div class="popup-header">
          <div class="drag-handle" @click.stop="toggleReservationPopupExpanded" @touchstart.stop="handleTouchStart"
            @touchmove.stop="handleTouchMove" @touchend.stop="handleTouchEnd" @mousedown.stop="handleMouseDown">
            <div class="drag-indicator"></div>
          </div>
          <h3>Reserve a bike</h3>
          <button class="close-btn" @click="closeReservationPopup">×</button>
        </div>
        <p class="bike-model">{{ selectedBike?.model }}</p>
        <div class="reservation-details">
          <div class="reservation-detail">
            <span>Price per minute:</span>
            <span>{{ selectedBike?.pricePerMinute }}€</span>
          </div>
          <div class="reservation-detail">
            <span>Reservation time:</span>
            <span>15 min.</span>
          </div>
        </div>
        <button class="begin-reservation-btn" @click="beginBikeReservation">
          <span class="icon">▶</span>
          Begin reservation
        </button>
      </div>
    </div>
  </div>
</template>

<script>
import L from 'leaflet'
import 'leaflet/dist/leaflet.css'
import { api } from '@/services/api-service'
import { authService } from '@/services/auth-service'

export default {
  name: 'MapView',
  data() {
    return {
      map: null,
      mapCenter: [54.6775530439872, 25.27774382871562],
      overlays: [],
      bikeIcon: null,
      zoneStyle: {
        color: '#3388ff',
        weight: 2,
        opacity: 0.7,
        fillOpacity: 0.2,
        fillColor: '#3388ff',
      },
      zones: [],
      isLoading: false,
      error: null,
      isLoggedIn: false,
      userEmail: null,
      showZonePopup: false,
      showReservationPopup: false,
      selectedZone: null,
      selectedBike: null,
      availableBikes: [],
      isZonePopupExpanded: false,
      isReservationPopupExpanded: false,
      isDragging: false,
      dragStartY: 0,
      dragCurrentY: 0,
      dragThreshold: 100,
      isMouseDragging: false,
    }
  },
  mounted() {
    this.initMap()
    this.createBikeIcon()
    this.checkAuthStatus()

    if (this.isLoggedIn) {
      this.fetchAndDisplayZones()
    }

    // Add global mouse event listeners
    document.addEventListener('mousemove', this.handleGlobalMouseMove)
    document.addEventListener('mouseup', this.handleGlobalMouseUp)
  },
  beforeUnmount() {
    // Clean up global listeners
    document.removeEventListener('mousemove', this.handleGlobalMouseMove)
    document.removeEventListener('mouseup', this.handleGlobalMouseUp)
  },
  methods: {
    checkAuthStatus() {
      this.isLoggedIn = authService.isTokenValid()
      this.userEmail = authService.getUserEmail()
    },

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
      alert('Click on the map to start drawing a zone. Double-click to finish.')

      const points = []
      let polygon = null
      let tempLine = null

      const handleMapClick = (e) => {
        points.push([e.latlng.lat, e.latlng.lng])

        if (points.length === 1) {
          tempLine = L.polyline(points, this.zoneStyle).addTo(this.map)
        } else {
          tempLine.setLatLngs(points)

          if (e.originalEvent.detail === 2 && points.length >= 3) {
            finishZoneDrawing()
          }
        }
      }

      const finishZoneDrawing = () => {
        this.map.off('click', handleMapClick)

        if (tempLine) {
          this.map.removeLayer(tempLine)
        }

        if (points.length >= 3) {
          polygon = L.polygon(points, this.zoneStyle).addTo(this.map)

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

          polygon.on('popupopen', () => {
            document.querySelectorAll('.color-option').forEach((option) => {
              option.addEventListener('click', (e) => {
                const color = e.target.getAttribute('data-color')
                polygon.setStyle({
                  color: color,
                  fillColor: color,
                })
              })
            })

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

      this.map.on('click', handleMapClick)
    },

    clearOverlays() {
      this.overlays.forEach((overlay) => {
        this.map.removeLayer(overlay)
      })
      this.overlays = []
    },

    async fetchAndDisplayZones() {
      try {
        this.isLoading = true
        this.error = null
        const response = await api.getZones()
        this.zones = response.data
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
        const bounds = [
          [zone.latitude1, zone.longitude1],
          [zone.latitude2, zone.longitude2],
        ]

        const rectangle = L.rectangle(bounds, {
          color: '#3388ff',
          weight: 2,
          opacity: 0.7,
          fillOpacity: 0.2,
          fillColor: '#3388ff',
        }).addTo(this.map)

        const availableBikes = zone.bikes ? zone.bikes.filter(bike => bike.status === 'Available') : []

        rectangle.bindTooltip(`${zone.name}<br>Available bikes: ${availableBikes.length}`, {
          permanent: true,
          direction: 'center',
          className: 'zone-label',
        })

        rectangle.on('click', () => {
          this.showZoneBikes(zone, availableBikes)
        })

        this.overlays.push(rectangle)
      })

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

    showZoneBikes(zone, availableBikes) {
      this.selectedZone = zone
      this.availableBikes = availableBikes
      this.showZonePopup = true
      this.isZonePopupExpanded = false
    },

    closeZonePopup() {
      this.showZonePopup = false
      this.selectedZone = null
      this.availableBikes = []
      this.isZonePopupExpanded = false
    },

    selectBike(bike) {
      this.selectedBike = bike
      this.showZonePopup = false
      this.showReservationPopup = true
      this.isReservationPopupExpanded = false
    },

    closeReservationPopup() {
      this.showReservationPopup = false
      this.selectedBike = null
      this.isReservationPopupExpanded = false
    },

    toggleZonePopupExpanded() {
      this.isZonePopupExpanded = !this.isZonePopupExpanded
    },

    toggleReservationPopupExpanded() {
      this.isReservationPopupExpanded = !this.isReservationPopupExpanded
    },

    handleTouchStart(event) {
      this.isDragging = true
      this.dragStartY = event.touches[0].clientY
      this.dragCurrentY = this.dragStartY
    },

    handleTouchMove(event) {
      if (!this.isDragging) return
      event.preventDefault()
      this.dragCurrentY = event.touches[0].clientY

      const dragDistance = this.dragStartY - this.dragCurrentY
      // Optional: Add visual feedback during drag
      // You can add transform styles here if needed
    },

    handleTouchEnd() {
      if (!this.isDragging) return
      this.isDragging = false

      const dragDistance = this.dragStartY - this.dragCurrentY
      const threshold = this.dragThreshold

      if (Math.abs(dragDistance) > threshold) {
        if (dragDistance > 0) {
          // Dragged up - expand
          if (this.showZonePopup) this.isZonePopupExpanded = true
          if (this.showReservationPopup) this.isReservationPopupExpanded = true
        } else {
          // Dragged down - collapse
          if (this.showZonePopup) this.isZonePopupExpanded = false
          if (this.showReservationPopup) this.isReservationPopupExpanded = false
        }
      }

      this.resetDragState()
    },

    handleMouseDown(event) {
      this.isMouseDragging = true
      this.isDragging = true
      this.dragStartY = event.clientY
      this.dragCurrentY = this.dragStartY
      event.preventDefault()
    },

    handleGlobalMouseMove(event) {
      if (!this.isDragging || !this.isMouseDragging) return
      this.dragCurrentY = event.clientY

      const dragDistance = this.dragStartY - this.dragCurrentY
      // Optional: Add visual feedback during drag
      // You can add transform styles here if needed
    },

    handleGlobalMouseUp() {
      if (!this.isDragging || !this.isMouseDragging) return
      this.isDragging = false
      this.isMouseDragging = false

      const dragDistance = this.dragStartY - this.dragCurrentY
      const threshold = this.dragThreshold

      if (Math.abs(dragDistance) > threshold) {
        if (dragDistance > 0) {
          // Dragged up - expand
          if (this.showZonePopup) this.isZonePopupExpanded = true
          if (this.showReservationPopup) this.isReservationPopupExpanded = true
        } else {
          // Dragged down - collapse
          if (this.showZonePopup) this.isZonePopupExpanded = false
          if (this.showReservationPopup) this.isReservationPopupExpanded = false
        }
      }

      this.resetDragState()
    },

    resetDragState() {
      this.isDragging = false
      this.isMouseDragging = false
      this.dragStartY = 0
      this.dragCurrentY = 0
    },

    beginBikeReservation() {
      if (this.selectedBike) {
        alert(`Reservation started for bike: ${this.selectedBike.model} (ID: ${this.selectedBike.id})`)
        this.closeReservationPopup()
      }
    },
  },
}
</script>

<style scoped>
.map-container {
  display: flex;
  flex-direction: column;
  height: 100vh;
  position: relative;
}

.map-view {
  width: 100%;
  height: 500px;
  margin-bottom: 20px;
  flex: 1;
}

.controls {
  display: flex;
  gap: 10px;
  margin-bottom: 20px;
}

.auth-status {
  margin: 10px 0;
  padding: 10px;
  background-color: #f5f5f5;
  border-radius: 4px;
  color: #000;
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

.bottom-popup {
  position: fixed;
  bottom: 0;
  left: 0;
  right: 0;
  /* background: rgba(0, 0, 0, 0.5); */
  z-index: 10000;
  display: flex;
  align-items: flex-end;
  animation: fadeIn 0.3s ease-out;
}

.bottom-popup.full-screen {
  align-items: stretch;
}

.bottom-popup-content {
  background: white;
  width: 100%;
  border-radius: 20px 20px 0 0;
  padding: 20px;
  max-height: 70vh;
  overflow-y: auto;
  animation: slideUp 0.3s ease-out;
  transition: all 0.3s ease-out;
}

.bottom-popup-content.expanded {
  height: 100vh;
  max-height: 100vh;
  border-radius: 0;
}

.popup-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 16px;
  border-bottom: 1px solid #eee;
  padding-bottom: 12px;
  position: relative;
}

.drag-handle {
  position: absolute;
  top: -15px;
  left: 50%;
  transform: translateX(-50%);
  width: 60px;
  height: 30px;
  cursor: grab;
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1;
  padding: 10px;
}

.drag-handle:active {
  cursor: grabbing;
}

.drag-indicator {
  width: 40px;
  height: 4px;
  background-color: #ccc;
  border-radius: 2px;
  transition: background-color 0.2s, width 0.2s;
}

.drag-handle:hover .drag-indicator {
  background-color: #999;
  width: 50px;
}

.drag-handle:active .drag-indicator {
  background-color: #666;
}

.popup-header h3 {
  margin: 0;
  font-size: 20px;
  font-weight: 600;
  color: #333;
}

.close-btn {
  background: none;
  border: none;
  font-size: 24px;
  color: #666;
  cursor: pointer;
  padding: 0;
  width: 30px;
  height: 30px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 50%;
  transition: background-color 0.2s;
}

.close-btn:hover {
  background-color: #f0f0f0;
}

.zone-subtitle {
  margin: 0 0 8px 0;
  color: #666;
  font-size: 14px;
}

.bikes-count {
  margin: 0 0 20px 0;
  font-weight: 600;
  color: #00897B;
  font-size: 16px;
}

.bikes-container {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.bike-card {
  background-color: #f8f9fa;
  border-radius: 12px;
  padding: 16px;
  cursor: pointer;
  transition: all 0.2s ease;
  border: 1px solid #e9ecef;
}

.bike-card:hover {
  background-color: #e3f2fd;
  border-color: #2196f3;
  transform: translateY(-1px);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.bike-card-content {
  margin-bottom: 16px;
}

.bike-name {
  margin: 0 0 12px 0;
  font-size: 18px;
  font-weight: 600;
  color: #333;
  text-align: center;
}

.bike-details {
  background-color: white;
  border-radius: 8px;
  padding: 12px;
}

.bike-detail {
  display: flex;
  justify-content: space-between;
  font-size: 16px;
}

.bike-detail span:first-child {
  color: #666;
}

.price {
  font-weight: 600;
  color: #00897B;
}

.select-bike-btn {
  background-color: #00897B;
  color: white;
  border: none;
  border-radius: 12px;
  padding: 12px 20px;
  width: 100%;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: background-color 0.2s;
}

.select-bike-btn:hover {
  background-color: #00695C;
}

.select-bike-btn .icon {
  margin-right: 8px;
  font-size: 14px;
}

.no-bikes {
  margin: 20px 0;
}

.no-bikes-content {
  background-color: #f8f9fa;
  border-radius: 12px;
  padding: 40px 20px;
  text-align: center;
}

.no-bikes-content p {
  margin: 0;
  color: #666;
  font-style: italic;
}

.bike-model {
  font-weight: 600;
  font-size: 18px;
  margin-bottom: 20px;
  color: #333;
  text-align: center;
}

.reservation-details {
  margin-bottom: 30px;
  background-color: #f8f9fa;
  border-radius: 12px;
  padding: 16px;
}

.reservation-detail {
  display: flex;
  justify-content: space-between;
  margin-bottom: 12px;
  font-size: 16px;
}

.reservation-detail:last-child {
  margin-bottom: 0;
}

.reservation-detail span:first-child {
  color: #666;
}

.reservation-detail span:last-child {
  font-weight: 600;
  color: #333;
}

.begin-reservation-btn {
  background-color: #00897B;
  color: white;
  border: none;
  border-radius: 12px;
  padding: 16px 20px;
  width: 100%;
  font-size: 18px;
  font-weight: 600;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: background-color 0.2s;
}

.begin-reservation-btn:hover {
  background-color: #00695C;
}

.begin-reservation-btn .icon {
  margin-right: 8px;
  font-size: 16px;
}

@keyframes fadeIn {
  from {
    opacity: 0;
  }

  to {
    opacity: 1;
  }
}

@keyframes slideUp {
  from {
    transform: translateY(100%);
  }

  to {
    transform: translateY(0);
  }
}
</style>
