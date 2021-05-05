import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDrawer } from '@angular/material/sidenav';
import { Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';

@Component({
  selector: 'app-filter',
  templateUrl: './filter.component.html',
  styleUrls: ['./filter.component.scss']
})
export class FilterComponent implements OnInit {

  @Input() 
  inputDrawer!: MatDrawer;

  filterForm!: FormGroup;

  categories: string[] = ["pierwsza", "druga"];
  filteredCategories!: Observable<string[]>;
  
  constructor(private fb: FormBuilder) { }

  ngOnInit(): void {
    this.filterForm = this.fb.group({
      category: [''],
    });

    this.filteredCategories = this.filterForm.controls['category'].valueChanges.pipe(
      startWith(''),
      map(value => typeof value === 'string' ? value : ''),
      map(name => name ? this._filter(name) : this.categories.slice(0,10))
    );
  }

  get f() { return this.filterForm.controls; }

  CloseDrawer()
  {
    this.inputDrawer.close();
  }

  ClearCategory()
  {
    this.filterForm.patchValue({ category: '' });
  }

  private _filter(name: any): any {
    const filterValue = name.toLowerCase();

    return this.categories.filter(cat => cat.toLowerCase().indexOf(filterValue) > -1);
  }
}

