function html_devices(data)
{
	var element = document.createElement('div');
	var element_title = document.createElement('h2');
	element_title.textContent = "Devices";
	element.appendChild(element_title);
	element.appendChild(html_devices_table(data.devices));
	return element;
}



function html_devices_table(rows)
{
	assert(Array.isArray(rows));

	var element = document.createElement('table');
	var thead = element.createTHead();
	var tbody = element.createTBody();
	var td;
	var tr;
	var th;

	tr = thead.insertRow(-1);

	th = document.createElement('th'); 
	th.textContent = "Id";
	tr.appendChild(th);

	th = document.createElement('th'); 
	th.textContent = "Name";
	tr.appendChild(th);

	for (let i = 0; i < rows.length; ++i)
	{
		tr = thead.insertRow(-1);

		td = tr.insertCell(-1);
		td.textContent = rows[i].id;
		td = tr.insertCell(-1);
		td.textContent = rows[i].name;
	}

	return element;
}