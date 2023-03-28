import http from 'k6/http'
import { sleep, check, group } from 'k6'

const accept = 'text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9';
const acceptEncoding = 'gzip, deflate, br';
const contentType = 'application/x-www-form-urlencoded';

//Configure users
export const options = {
    stages: [
        { target: 20, duration: '30s' },
        { target: 20, duration: '5m'},
        { target: 0, duration: '10s'}
    ],
    thresholds: {
        http_req_failed:['rate<0.01'],
        'http_req_duration{type:command}':['p(99)<200'],
        'http_req_duration{type:query}':['p(99)<200']
    }
}

export function setup() {
    console.log("Wow! Test started");
    let baseurl = '';
    if (__ENV.BASEURL) {
        baseurl = __ENV.BASEURL
    }  
    return {'baseUrl': baseurl};
}

export default function(data) {
    group('account edition', function() { AccountEditionScenario(data)});
    group('invoice edition', function() { InvoiceEditionScenario(data)});
}

function AccountEditionScenario(data) {
    let response = http.get(`${data.baseUrl}`,
        { headers:{'Accept': accept, 'Accept-Encoding': acceptEncoding },
          tags:{type:'query'} });
    sleep(2);
    // Get companyCorrelationId
    const companyCorrelationId = GetCorrelationIdFromHtml(response.html(), 0, 'companyCorrelationId');

    response = http.get(`${data.baseUrl}/Accounts?companyCorrelationId=${companyCorrelationId}`,
        { headers:{'Accept': accept, 'Accept-Encoding': acceptEncoding },
          tags:{type:'query'}});
    sleep(2);

    //Get accountCorrelationId
    const accountCorrelationId = GetCorrelationIdFromHtml(response.html(), 0, 'accountCorrelationId');

    response = http.get(`${data.baseUrl}/Accounts/Edit?companyCorrelationId=${companyCorrelationId}&accountCorrelationId=${accountCorrelationId}`, 
        { headers:{'Accept': accept, 'Accept-Encoding': acceptEncoding },
          tags:{type:'query'}});
    sleep(2);
    //Get the form values
    const id = response.html().find('input[id="Id"]').attr('value');
    const accountCode = response.html().find('input[id="Code"]').attr('value');
    const description = CalculateNewDescription(response.html().find('input[id="Description"]').attr('value'));
    const requestVerificationToken = response.html().find('input[name="__RequestVerificationToken"]').attr('value');


    const payload = `CompanyCorrelationId=${companyCorrelationId}&CorrelationId=${accountCorrelationId}&Code=${accountCode}&Id=${id}&Description=${description}&__RequestVerificationToken=${requestVerificationToken}`;
    response = http.post(`${data.baseUrl}/Accounts/Edit`, payload, 
        { headers:{'Accept': accept, 'Accept-Encoding': acceptEncoding,'Content-Type': contentType },
          tags:{type:'command'}});
    
    check(response, {'Entity saved succesfully': (r) => r.status >= 200 && r.status <= 226});

    response = http.get(`${data.baseUrl}/Accounts/Details?companyCorrelationId=${companyCorrelationId}&accountCorrelationId=${accountCorrelationId}`,
        { headers:{'Accept': accept, 'Accept-Encoding': acceptEncoding },
          tags:{type:'query'}});
    check(response, { 'Description well saved': (r) => r.html().find('dd[class="col-sm-10"]').last().text().trim() == description })
}

function InvoiceEditionScenario(data) {
    // Get home page
    let response = http.get(`${data.baseUrl}`,
        { headers:{'Accept': accept, 'Accept-Encoding': acceptEncoding },
          tags:{type:'query'} });
    const companyCorrelationId = GetCorrelationIdFromHtml(response.html(), 1, "companyCorrelationId");
    sleep(2);
    
    // Navigate to accounts list  
    response = http.get(`${data.baseUrl}/Invoices?companyCorrelationId=${companyCorrelationId}`,
        { headers:{'Accept': accept, 'Accept-Encoding': acceptEncoding },
          tags:{type:'query'}});
    
    const invoiceCorrelationId = GetCorrelationIdFromHtml(response.html(), 1, "invoiceCorrelationId");
    sleep(2)
    
    // Navigate to account edition
    response = http.get(`${data.baseUrl}/Invoices/Edit?companyCorrelationId=${companyCorrelationId}&invoiceCorrelationId=${invoiceCorrelationId}`,
        { headers:{'Accept': accept, 'Accept-Encoding': acceptEncoding },
          tags:{type:'query'}});
    sleep(2)
     
    let id = response.html().find('input[id="Id"]').attr('value');
    let accountCode = response.html().find('input[id="AccountCode"]').attr('value');
    let concept = CalculateNewDescription(response.html().find('input[id="Concept"]').attr('value'));
    let amount = response.html().find('input[id="Amount"]').attr('value');
    let taxAmount = response.html().find('input[id="TaxAmount"]').attr('value');
    const requestVerificationToken = response.html().find('input[name="__RequestVerificationToken"]').attr('value');
    
    // Edit invoice   
    const PAYLOAD = `CompanyCorrelationId=${companyCorrelationId}&CorrelationId=${invoiceCorrelationId}&Id=${id}&Concept=${concept}&AccountCode==${accountCode}&Amount=${amount}&TaxAmount=${taxAmount}}&__RequestVerificationToken=${requestVerificationToken}`;
    response = http.post(`${data.baseUrl}/Invoices/Edit`, PAYLOAD, 
        { headers: {'Accept': accept, 'Accept-Encoding': acceptEncoding, 'Content-Type': contentType },
          tags:{type:'command'}});
    check(response, { 'Entity edition with estatus code 200': (r) => r.status >= 200 && r.status <= 226 })
    
    // check value is edited    
    response = http.get(`${data.baseUrl}/Invoices/Details?companyCorrelationId=${companyCorrelationId}&invoiceCorrelationId=${invoiceCorrelationId}`,
        { headers:{'Accept': accept, 'Accept-Encoding': acceptEncoding },
        tags:{type:'query'}});
    
        check(response, { 'Description well saved': (r) => r.html().find('dd[class="col-sm-10"]').toArray()[1].text().trim() == concept })
}
    


export function teardown(data) {
    console.log("Yeah! Test finished");
}

function GetCorrelationIdFromHtml(body, option, name ) {
    // Get the rows
    const rows = body.find('tbody').children("tr").toArray();
    // calculate row index
    let index = __VU - 1;
    if (__VU > rows.length)
        index = (__VU % rows.length); 
    // Get the column with options
    const optionsColumns = rows[index].last('td').find('a').toArray();
    // Get the href from accounts (first option)
    const accountResource = optionsColumns[option].attr('href');
    
    const initialPosition = accountResource.indexOf(`${name}=`) + `${name}=`.length;
    //extract id with name 
    return accountResource.substring(initialPosition , initialPosition + 36); 
}

// Random Description
function CalculateNewDescription(description) {
    const position = description.indexOf("_");
    if (position == -1)
        return description + `_${generateCodeFromDate()}`;
    else
    return description.substring(0, position) + `_${generateCodeFromDate()}`;
}
    
//Random value
function generateCodeFromDate() {
    let date = new Date();
     return date.valueOf()
}
