import { Injectable } from '@angular/core';

import { HttpClient, HttpHeaders } from '@angular/common/http';

// import { Observable } from 'rxjs/Observable';

import { Observable } from 'rxjs';

import { environment } from '../../environments/environment';

import { Template } from '../models/template.model';

import { Globals } from '../globals';

@Injectable()
export class TemplatesService {
  private readonly apiTemplatesUrl = '/api/Templates';
  private readonly headers = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json'
    })
  };

  constructor(
    private globals: Globals,
    private httpClient: HttpClient
  ) { }

  getTemplates(skip: number): Observable<Template[]> {
    const url = this.apiTemplatesUrl;
    return this.httpClient.get<Template[]>(url, this.headers);
  }

  getTemplatesByTypeAndByYear(type: string, year: string, skip: number): Observable<Template[]> {
    const url = this.apiTemplatesUrl + `/${type}/${year}`;
    return this.httpClient.get<Template[]>(url, this.headers);
  }

  putTemplatesByTypeAndByYear(templateData: FormData): Observable<any> {
    const reqBodyObj = this.serializeTemplateRequestBodyObject(templateData);
    const url = this.apiTemplatesUrl +
      `/${templateData.get('type')}/${templateData.get('year')}?content=${templateData}`;
    return this.httpClient.put<Template>(url, reqBodyObj);
  }

  private serializeTemplateRequestBodyObject(templateData: FormData): Object {
    Object.keys(templateData).forEach(
      k => {
        let v = templateData[k];
        if (typeof(v) === 'object') {
          templateData.set(k, JSON.stringify(v));
        }
        else {
          templateData.set(k, v);
        }
      }
    )

    return templateData;
  }

}
