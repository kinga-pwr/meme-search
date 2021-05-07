import { stringify } from '@angular/compiler/src/util';
import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatChip, MatChipList } from '@angular/material/chips';
import { MatDrawer } from '@angular/material/sidenav';
import { Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';
import { Category } from '../models/category';
import { Source } from '../models/source';
import { InformationService } from '../services/information.service';

@Component({
    selector: 'app-filter',
    templateUrl: './filter.component.html',
    styleUrls: ['./filter.component.scss']
})
export class FilterComponent implements OnInit {


    @Input() inputDrawer!: MatDrawer;
    @ViewChild('chipList') chipList!: MatChipList;
    categories: Category[] = [];
    filteredCategories!: Observable<Category[]>;
    filteredSources!: Observable<Source[]>;
    filterForm!: FormGroup;
    filters = ["Status: Confirmed", "Status: Submitted", "Status: Deadpool"];
    sources: Source[] = [];
    statusChips: any = [];
    years: any = { lower: 1968, upper: 2021 };
    sliderrange: any;

    constructor(private fb: FormBuilder, private infoService: InformationService) {
        infoService.Categories().subscribe(
            data => {
                this.categories = data;
            },
            error => { console.log("error") }
        );

        infoService.Source().subscribe(
            data => {
                this.sources = data;
            },
            error => { console.log("error") }
        );

        infoService.Statuses().subscribe(
            data => {
                data.forEach(status => this.statusChips.push({name: status, selected: true}));
            },
            error => { console.log("error") }
        );
    }

    ngOnInit(): void {
        this.filterForm = this.fb.group({
            category: [''],
            source: [''],
            statuses: [[...this.statusChips], Validators.required]
        });

        this.filteredCategories = this.filterForm.controls['category'].valueChanges.pipe(
            startWith(''),
            map(value => typeof value === 'string' ? value : ''),
            map(name => name ? this._filter(this.categories, name) : this.categories.slice(0, 10))
        );

        this.filteredSources = this.filterForm.controls['source'].valueChanges.pipe(
            startWith(''),
            map(value => typeof value === 'string' ? value : ''),
            map(name => name ? this._filter(this.sources, name) : this.sources.slice(0, 10))
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

    ClearSource() {
        this.filterForm.patchValue({ source: '' });
    }

    ToggleSelection(chip: MatChip) {
        chip.toggleSelected();
        var current: string[] = this.filterForm.get('statuses')!.value;

        if (chip.selected)
            current.push(chip.value);
        else {
            var idx = current.indexOf(chip.value);
            current.splice(idx, 1);
        }

        this.filterForm.controls.statuses.setValue(current);
    }

    Remove(filter: string) {
        var idx = this.filters.indexOf(filter);
        this.filters.splice(idx, 1);
    }

    DisplayCategory(cat: Category): string {
      return cat ? `${cat.name} (${cat.quantity})` : "";
    }

    DisplaySource(source: Source): string {
      return source ? `${source.name} (${source.quantity})` : "";
    }

    Filter()
    {
    }

    private _filter(list: any, name: any): any {
        const filterValue = name.toLowerCase();

        return list.filter((item: any)=> item.name.toLowerCase().indexOf(filterValue) > -1);
    }
}

