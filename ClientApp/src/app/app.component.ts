import { Component, Inject } from '@angular/core';
import {HttpClient } from '@angular/common/http'


@Component({
    selector: 'app-root',
    templateUrl: './app.component.html'
})
export class AppComponent {
  options: {};

  markerSize = 25;
  public mData: ModelData[];
  public mins: ModelData[];
  public maxs: ModelData[];
  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<ModelData[][]>(baseUrl + 'weatherforecast').subscribe(result => {
      this.mData = result[0];
      this.mins = result[1];
      this.maxs = result[2];
      console.log(this.mData);
    //  this.setMarkerSize(this.markerSize, this.mData);
      this.setMarkerSize(this.markerSize, this.mData);
   }, error => console.error(error));

    this.setMarkerSize(this.markerSize, []);
    //this.setMarkerSize(this.markerSize, []);
     //   this.updateOptions(this.mData);
    }

  //constructor() {
  //  this.updateOptions();
  //}

    setMarkerSize(markerSize: number, arr) {
        this.markerSize = markerSize;
        this.updateOptions(arr);
    }

  updateOptions(r) {
    //var foo = new Foo();
    //var _arr = [{
    //  value: 1.515515,
    //  dateTime:'04/01/1950T20:11:01'
    //},
    //  {
    //  {
    //    value: 1.61715,
    //    dateTime: '04/01/1951T20:11:01'
    //  },
    //  {
    //    value: 1.991341,
    //    dateTime: '04/01/1952T20:11:01'
    //  },
    //  {
    //    value: 1.91145,
    //    dateTime: '04/01/1953T20:11:01'
    //  },
    //  {
    //    value: 2.11145,
    //    dateTime: '04/01/1954T20:11:01'
    //  },
    //  {
    //    value: 1.81451345,
    //    dateTime: '04/01/1955T20:11:01'
    //  },
    //  {
    //    value: 1.71832,
    //    dateTime: '04/01/1956T20:11:01'
    //  },
    //  {
    //    value: 1.5151513515,
    //    dateTime: '04/01/1957T20:11:01'
    //  },
    //  {
    //    value: 1.5713145,
    //    dateTime: '04/01/1958T20:11:01'
    //  },
    //  {
    //    value: 1.991341,
    //    dateTime: '04/01/1959T20:11:01'
    //  },
    //  {
    //    value: 1.59151245,
    //    dateTime: '04/01/1960T20:11:01'
    //  },
    //  {
    //    value: 2.001345,
    //    dateTime: '04/01/1990T20:11:01'
    //  },
    //  {
    //    value: 1.6714451345,
    //    dateTime: '04/01/1982T20:11:01'
    //  },
    //  {
    //    value: 1.71832,
    //    dateTime: '04/01/1999T20:11:01'
    //  }, {
    //    value: 1.875615,
    //    dateTime: '04/01/1976T20:11:01'
    //  }
      
    //];

        this.options = {
          //  data: [{
          //      month: 'Jan',
          //      revenue: 17000,
          //      profit: 33000
          //  }, {
          //      month: 'Feb',
          //      revenue: 123000,
          //      profit: 35500
          //  }, {
          //      month: 'Mar',
          //      revenue: 172500,
          //      profit: 41000
          //  }, {
          //      month: 'Apr',
          //      revenue: 185000,
          //      profit: 50000
          //  }],
          //  series: [{
          //      xKey: 'month',
          //      yKey: 'revenue'
          //}]
          //    data: [foo.fillFromJSON(this.mData)]
          data: r
          ,
          series: [{
              xKey: 'dateTime',
               yKey: 'value'
          }],
            legend: {
                markerSize: this.markerSize
            }
        };
    }
}
interface ModelData {
  value: number,
  dateTime:Date
}

interface ModelArray {
  arr: ModelData[]
}
class Serializable {
  fillFromJSON(json: string) {
    var jsonObj = JSON.parse(json);
    for (var propName in jsonObj) {
      this[propName] = jsonObj[propName]
    }
  }
}

class Foo extends Serializable {
  name: string;
  GetName(): string { return this.name }
}




