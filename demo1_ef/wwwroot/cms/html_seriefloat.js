function html_seriefloats(data)
{
	var element = document.createElement('div');
	var element_title = document.createElement('h2');
	element_title.textContent = "Seriefloats";
	element.appendChild(element_title);
	element.appendChild(html_seriefloats_table(data.floatvals));
	return element;
}



function html_seriefloats_table(rows)
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
	th.textContent = "Time";
	tr.appendChild(th);

	th = document.createElement('th'); 
	th.textContent = "Value";
	tr.appendChild(th);

	for (let i = 0; i < rows.length; ++i)
	{
		tr = thead.insertRow(-1);

		td = tr.insertCell(-1);
		td.textContent = rows[i].time;
		td = tr.insertCell(-1);
		td.textContent = rows[i].value;
	}

	return element;
}