async function getJson(url) {
  const response = await fetch(url, { headers: { Accept: "application/json" } });
  if (!response.ok) {
    return [];
  }

  return response.json();
}

function fillSelect(select, placeholder, values) {
  select.innerHTML = "";

  const placeholderOption = document.createElement("option");
  placeholderOption.value = "";
  placeholderOption.textContent = placeholder;
  select.appendChild(placeholderOption);

  values.forEach((value) => {
    const option = document.createElement("option");
    option.value = value;
    option.textContent = value;
    select.appendChild(option);
  });
}

function syncStartLocation(picker) {
  const form = picker.closest("form");
  const city = picker.querySelector("[data-city-select]").value;
  const district = picker.querySelector("[data-district-select]").value;
  const neighborhood = picker.querySelector("[data-neighborhood-select]").value;
  const address = picker.querySelector("[data-address-input]")?.value;

  form.elements.startLocation.value = [city, district, neighborhood, address].filter(Boolean).join(" ");
}

async function loadDistricts(province, districtSelect, selectedDistrict) {
  const districts = province
    ? await getJson(`/Home/Districts?province=${encodeURIComponent(province)}`)
    : [];

  fillSelect(districtSelect, province ? "İlçe seç" : "Önce il seç", districts);
  districtSelect.disabled = districts.length === 0;
  districtSelect.value = selectedDistrict || "";

  return districtSelect.value;
}

async function loadNeighborhoods(province, district, neighborhoodSelect, selectedNeighborhood) {
  const neighborhoods = province && district
    ? await getJson(`/Home/Neighborhoods?province=${encodeURIComponent(province)}&district=${encodeURIComponent(district)}`)
    : [];

  fillSelect(neighborhoodSelect, district ? "Mahalle seç" : "Önce ilçe seç", neighborhoods);
  neighborhoodSelect.disabled = neighborhoods.length === 0;
  neighborhoodSelect.value = selectedNeighborhood || "";
}

document.querySelectorAll("[data-location-picker]").forEach(async (picker) => {
  const citySelect = picker.querySelector("[data-city-select]");
  const districtSelect = picker.querySelector("[data-district-select]");
  const neighborhoodSelect = picker.querySelector("[data-neighborhood-select]");
  const selectedCity = citySelect.dataset.selected || "";
  const selectedDistrict = districtSelect.dataset.selected || "";
  const selectedNeighborhood = neighborhoodSelect.dataset.selected || "";

  const provinces = await getJson("/Home/Provinces");
  fillSelect(citySelect, "İl seç", provinces);
  citySelect.value = selectedCity;

  const district = await loadDistricts(citySelect.value, districtSelect, selectedDistrict);
  await loadNeighborhoods(citySelect.value, district, neighborhoodSelect, selectedNeighborhood);
  syncStartLocation(picker);

  citySelect.addEventListener("change", async () => {
    const district = await loadDistricts(citySelect.value, districtSelect, "");
    await loadNeighborhoods(citySelect.value, district, neighborhoodSelect, "");
    syncStartLocation(picker);
  });

  districtSelect.addEventListener("change", async () => {
    await loadNeighborhoods(citySelect.value, districtSelect.value, neighborhoodSelect, "");
    syncStartLocation(picker);
  });

  neighborhoodSelect.addEventListener("change", () => syncStartLocation(picker));
  picker.querySelector("[data-address-input]")?.addEventListener("input", () => syncStartLocation(picker));
});

document.querySelectorAll(".place-card__checkbox").forEach((checkbox) => {
  const syncCardState = () => {
    checkbox.closest(".place-card")?.classList.toggle("place-card--selected", checkbox.checked);
  };

  syncCardState();
  checkbox.addEventListener("change", syncCardState);
});

document.querySelectorAll("[data-route-type-picker]").forEach((picker) => {
  const form = picker.closest("form");
  const travelDaysSelect = form?.querySelector("[data-travel-days-select]");
  const roundTripDaysHint = form?.querySelector("[data-round-trip-days-hint]");
  const syncRouteTypeState = () => {
    const selectedRouteType = picker.querySelector('input[type="radio"]:checked')?.value;
    const isRoundTrip = selectedRouteType === "roundTrip";

    picker.querySelectorAll(".route-type-option").forEach((option) => {
      const input = option.querySelector('input[type="radio"]');
      option.classList.toggle("route-type-option--selected", input?.checked === true);
    });

    if (travelDaysSelect) {
      travelDaysSelect.value = isRoundTrip ? "1" : travelDaysSelect.value;
      travelDaysSelect.disabled = isRoundTrip;
    }

    if (roundTripDaysHint) {
      roundTripDaysHint.hidden = !isRoundTrip;
    }
  };

  syncRouteTypeState();
  picker.querySelectorAll('input[type="radio"]').forEach((input) => {
    input.addEventListener("change", syncRouteTypeState);
  });
});

document.querySelectorAll("[data-clear-plan]").forEach((button) => {
  button.addEventListener("click", async () => {
    const form = button.closest("form");
    const layout = button.closest(".planner-layout");
    const sidebar = layout?.querySelector("[data-plan-sidebar]");
    const selectedStartLocation = document.querySelector("[data-selected-start-location]");
    const picker = form.querySelector("[data-location-picker]");
    const citySelect = picker.querySelector("[data-city-select]");
    const districtSelect = picker.querySelector("[data-district-select]");
    const neighborhoodSelect = picker.querySelector("[data-neighborhood-select]");
    const addressInput = picker.querySelector("[data-address-input]");

    const provinces = await getJson("/Home/Provinces");
    fillSelect(citySelect, "İl seç", provinces);
    fillSelect(districtSelect, "Önce il seç", []);
    fillSelect(neighborhoodSelect, "Önce ilçe seç", []);
    districtSelect.disabled = true;
    neighborhoodSelect.disabled = true;

    form.elements.startLocation.value = "";
    form.querySelector("[data-start-location-input]")?.setAttribute("value", "");
    const cityRouteType = form.querySelector('input[name="routeType"][value="city"]');
    if (cityRouteType) {
      cityRouteType.checked = true;
      cityRouteType.dispatchEvent(new Event("change", { bubbles: true }));
    }
    if (addressInput) {
      addressInput.value = "";
    }
    selectedStartLocation?.remove();

    form.querySelectorAll('input[name="selectedPlaceIds"]').forEach((checkbox) => {
      checkbox.checked = false;
      checkbox.closest(".place-card").classList.remove("place-card--selected");
    });

    if (sidebar) {
      sidebar.innerHTML = `
        <div class="plan-card" data-empty-plan>
          <span class="eyebrow">Plan Bekleniyor</span>
          <h3>Önce kartlardan seçim yap</h3>
          <p>Gitmek istediğin yerleri işaretledikten sonra plan oluştur butonuna basman yeterli.</p>
        </div>
      `;
    }
  });
});

const routeDayColors = ["#06b6d4", "#ec4899", "#84cc16", "#f97316", "#8b5cf6", "#64748b"];

function createRouteMarkerIcon(stop, color) {
  return L.divIcon({
    className: "route-pin",
    html: `<span style="--route-pin-color: ${color}">${stop.order}</span>`,
    iconSize: [34, 34],
    iconAnchor: [17, 17],
    popupAnchor: [0, -18]
  });
}

function createStartMarkerIcon() {
  return L.divIcon({
    className: "route-pin route-pin--start",
    html: "<span>B</span>",
    iconSize: [34, 34],
    iconAnchor: [17, 17],
    popupAnchor: [0, -18]
  });
}

function renderRouteLegend(container, days) {
  container.innerHTML = "";

  days.forEach((day, index) => {
    const color = routeDayColors[index % routeDayColors.length];
    const item = document.createElement("span");
    item.className = "route-map-legend__item";
    item.innerHTML = `<i style="background: ${color}"></i>${day.dayNumber}. G\u00fcn - ${day.distanceKm} km`;
    container.appendChild(item);
  });
}

function initializeRouteMap(panel) {
  if (panel.dataset.mapReady === "true" || !window.L) {
    return;
  }

  const mapElement = panel.querySelector("[data-route-map]");
  const legendElement = panel.querySelector("[data-route-map-legend]");
  const jsonElement = document.querySelector("[data-route-map-json]");
  const payload = JSON.parse(jsonElement?.textContent || "{}");
  const bounds = [];
  const map = L.map(mapElement, { scrollWheelZoom: false });

  L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
    maxZoom: 19,
    attribution: "&copy; OpenStreetMap"
  }).addTo(map);

  if (payload.start?.latitude && payload.start?.longitude) {
    const startPoint = [payload.start.latitude, payload.start.longitude];
    bounds.push(startPoint);
    L.marker(startPoint, { icon: createStartMarkerIcon() })
      .bindPopup(`<strong>Ba\u015flang\u0131\u00e7</strong><br>${payload.start.name || ""}`)
      .addTo(map);
  }

  payload.days?.forEach((day, dayIndex) => {
    const color = routeDayColors[dayIndex % routeDayColors.length];
    const points = day.stops.map((stop) => [stop.latitude, stop.longitude]);
    const dayLinePoints = [...points];

    if (dayIndex === 0 && payload.start?.latitude && payload.start?.longitude) {
      dayLinePoints.unshift([payload.start.latitude, payload.start.longitude]);
    }

    if (dayLinePoints.length > 1) {
      L.polyline(dayLinePoints, {
        color,
        weight: 5,
        opacity: 0.86,
        lineJoin: "round"
      }).addTo(map);
    }

    day.stops.forEach((stop) => {
      const point = [stop.latitude, stop.longitude];
      bounds.push(point);
      L.marker(point, { icon: createRouteMarkerIcon(stop, color) })
        .bindPopup(`<strong>${stop.order}. ${stop.name}</strong><br>${day.dayNumber}. G\u00fcn<br>${stop.category} - ${stop.estimatedVisitDuration}`)
        .addTo(map);
    });
  });

  const lastDay = payload.days?.[payload.days.length - 1];
  const lastStop = lastDay?.stops?.[lastDay.stops.length - 1];
  if (payload.routeType === "roundTrip" && payload.start?.latitude && payload.start?.longitude && lastStop) {
    L.polyline(
      [
        [lastStop.latitude, lastStop.longitude],
        [payload.start.latitude, payload.start.longitude]
      ],
      {
        color: "#111827",
        weight: 4,
        opacity: 0.72,
        dashArray: "8 8",
        lineJoin: "round"
      }
    ).addTo(map);
  }

  renderRouteLegend(legendElement, payload.days || []);

  if (bounds.length > 0) {
    map.fitBounds(bounds, { padding: [28, 28], maxZoom: 14 });
  } else {
    map.setView([39, 35], 6);
  }

  panel.dataset.mapReady = "true";
  panel.routeMap = map;
}

document.querySelectorAll("[data-show-route-map]").forEach((button) => {
  button.addEventListener("click", () => {
    const panel = button.closest("[data-active-plan]")?.querySelector("[data-route-map-panel]");
    if (!panel) {
      return;
    }

    panel.hidden = !panel.hidden;
    button.textContent = panel.hidden ? "Haritada G\u00f6r" : "Haritay\u0131 Gizle";

    if (!panel.hidden) {
      initializeRouteMap(panel);
      setTimeout(() => panel.routeMap?.invalidateSize(), 50);
      panel.scrollIntoView({ behavior: "smooth", block: "nearest" });
    }
  });
});
