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
