function getTodayString() {
      const now = new Date();
      return String(now.getDate()).padStart(2, '0') +
             String(now.getMonth() + 1).padStart(2, '0') +
             now.getFullYear();
    }

    function downloadXML(content, fileName) {
      const blob = new Blob([content], { type: 'application/xml' });
      const url = URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = fileName;
      a.click();
      URL.revokeObjectURL(url);
    }

    function generateXIE() {
      const tech = document.getElementById('technology').value.trim();
      const path = document.getElementById('folder').value.trim();
      const serials = document.getElementById('serialList').value.trim().split('\n').filter(Boolean);
      const parts = path.split('/').filter(Boolean);
      if (parts.length < 2 || !tech || serials.length === 0) return alert('Completa todos los campos de Altas.');

      const sponsor = parts[0];
      const folders = parts.slice(1);
      let xml = `<?xml version = "1.0" encoding="utf-8"?>\n<script version = "1.0" xmlns:xsi = "http://www.w3.org/2001/XMLSchema-instance">\n  <context>\n    <estate signature = "${sponsor}">\n    </estate>\n  </context>\n`;

      folders.forEach((f, i) => xml += `${'  '.repeat(i + 1)}<estate signature = "${f}">\n`);
      serials.forEach(s => xml += `${'  '.repeat(folders.length + 1)}<terminal signature = "${s}"><status value = "ENABLED"/><type value = "${tech}"/></terminal>\n`);
      for (let i = folders.length - 1; i >= 0; i--) xml += `${'  '.repeat(i + 1)}</estate>\n`;
      xml += '</script>';

      downloadXML(xml, `Altas ${getTodayString()}.xie`);
    }

    function generateDeleteXIE() {
      const path = document.getElementById('folderDelete').value.trim();
      const serials = document.getElementById('serialListDelete').value.trim().split('\n').filter(Boolean);
      const parts = path.split('/').filter(Boolean);
      if (parts.length < 2 || serials.length === 0) return alert('Completa todos los campos de Bajas.');

      const sponsor = parts[0];
      const folders = parts.slice(1);
      let xml = `<?xml version = "1.0" encoding="utf-8"?>\n<script version = "1.0" xmlns:xsi = "http://www.w3.org/2001/XMLSchema-instance">\n  <context>\n    <estate signature = "${sponsor}">\n    </estate>\n  </context>\n`;

      folders.forEach((f, i) => xml += `${'  '.repeat(i + 1)}<estate signature = "${f}">\n`);
      serials.forEach(s => xml += `${'  '.repeat(folders.length + 1)}<deleteTerminal signature = "${s}"/>\n`);
      for (let i = folders.length - 1; i >= 0; i--) xml += `${'  '.repeat(i + 1)}</estate>\n`;
      xml += '</script>';

      downloadXML(xml, `Bajas ${getTodayString()}.xie`);
    }

    function generateCampaignXIE() {
      const sponsor = document.getElementById('campaignSponsor').value.trim();
      const name = document.getElementById('campaignName').value.trim();
      const type = document.getElementById('campaignType').value;
      const serials = document.getElementById('campaignSeries').value.trim().split('\n').filter(Boolean);
      if (!sponsor || !name || !type || serials.length === 0) return alert('Completa todos los campos de Campañas.');

      let xml = `<?xml version = "1.0" encoding = "utf-8"?>\n<script version = "1.0" xmlns:xsi = "http://www.w3.org/2001/XMLSchema-instance">\n  <context>\n    <estate signature = "${sponsor}">\n    </estate>\n  </context>\n\n  <campaign type= "${type}" name = "${name}">\n`;
      serials.forEach(s => xml += `    <target signature = "${s}"/>\n`);
      xml += '  </campaign>\n</script>';

      downloadXML(xml, `Campaña ${getTodayString()}.xie`);
    }

    function generateDeleteFromCampaignXIE() {
      const sponsor = document.getElementById('campaignSponsor').value.trim();
      const name = document.getElementById('campaignName').value.trim();
      const type = document.getElementById('campaignType').value;
      const serials = document.getElementById('campaignSeries').value.trim().split('\n').filter(Boolean);
      if (!sponsor || !name || !type || serials.length === 0) return alert('Completa todos los campos para eliminar de la campaña.');

      let xml = `<?xml version = "1.0" encoding = "utf-8"?>\n<script version = "1.0" xmlns:xsi = "http://www.w3.org/2001/XMLSchema-instance">\n  <context>\n    <estate signature = "${sponsor}">\n    </estate>\n  </context>\n\n  <campaign type= "${type}" name = "${name}">\n`;
      serials.forEach(s => xml += `    <deleteTarget signature = "${s}"/>\n`);
      xml += '  </campaign>\n</script>';

      downloadXML(xml, `EliminarCampaña ${getTodayString()}.xie`);
}

function generateAdvancedConfigXIE() {
  const fileInput = document.getElementById('csvFile');
  if (!fileInput.files.length) {
    alert('Selecciona un archivo CSV.');
    return;
  }

  const reader = new FileReader();
  reader.onload = function (e) {
    const content = e.target.result;
    const delimiter = content.includes(';') ? ';' : ',';
    const rows = content.trim().split(/\r?\n/);
    const headers = rows[0].split(delimiter).map(h => h.trim().toLowerCase());

    const rutaIdx = headers.indexOf("rutacompleta");
    const serieIdx = headers.indexOf("serie");
    const nameIdx = headers.indexOf("terminal");
    const merchIdx = headers.indexOf("comercio");

    if ([rutaIdx, serieIdx, nameIdx, merchIdx].includes(-1)) {
      alert('El archivo debe tener las columnas: RutaCompleta, Serie, Terminal, Comercio.');
      return;
    }

    let sponsor = "";
    const structure = {};

    for (let i = 1; i < rows.length; i++) {
      const cols = rows[i].split(delimiter).map(c => c.trim());
      const fullPath = cols[rutaIdx];
      const serie = cols[serieIdx];
      const name = cols[nameIdx].padStart(11, '0');
      const merchant = cols[merchIdx].padStart(9, '0');

      const parts = fullPath.split('/').filter(Boolean);
      if (!sponsor) sponsor = parts[0]; // primer elemento como sponsor
      const path = parts.slice(1); // resto como estates

      let current = structure;
      for (const part of path) {
        if (!current[part]) current[part] = {};
        current = current[part];
      }

      if (!current.terminals) current.terminals = [];
      current.terminals.push({ serie, name, merchant });
    }

    let xml = `<?xml version = "1.0" encoding = "utf-8"?>\n<script version = "1.0" xmlns:xsi = "http://www.w3.org/2001/XMLSchema-instance">\n`;
    xml += `  <context>\n    <estate signature="${sponsor}">\n    </estate>\n  </context>\n\n`;

    function buildEstate(obj, path = []) {
      for (const key in obj) {
        if (key === 'terminals') {
          const tabs = '    '.repeat(path.length + 1);
          obj[key].forEach(t => {
            xml += `${tabs}<terminal signature="${t.serie}">\n`;
            xml += `${tabs}  <name value="${t.name}"/>\n`;
            xml += `${tabs}  <merchantId value="${t.merchant}"/>\n`;
            xml += `${tabs}  <parameterSet templateId="cestrack" templateVersion="00101">\n`;
            xml += `${tabs}    <parameter fieldId="MERCHANT_ID" value="${t.merchant}"/>\n`;
            xml += `${tabs}    <parameter fieldId="LTID" value="${t.name}"/>\n`;
            xml += `${tabs}  </parameterSet>\n`;
            xml += `${tabs}</terminal>\n`;
          });
        } else {
          const tabs = '    '.repeat(path.length);
          xml += `${tabs}<estate signature="${key}">\n`;
          buildEstate(obj[key], path.concat(key));
          xml += `${tabs}</estate>\n`;
        }
      }
    }

    buildEstate(structure);
    xml += '</script>';

    downloadXML(xml, `Configuración ${getTodayString()}.xie`);
  };

  reader.readAsText(fileInput.files[0]);
}
