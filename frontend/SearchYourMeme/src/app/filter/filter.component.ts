import { stringify } from '@angular/compiler/src/util';
import { AfterViewInit, Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatChip } from '@angular/material/chips';
import { MatDrawer } from '@angular/material/sidenav';
import { Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';
import { Category } from '../models/category';
import { QueryParams } from '../models/query-params.interface';
import { Source } from '../models/source';
import { AdvancedSearchService } from '../services/advanced-search.service';
import { ImageFiltersService } from '../services/image-filters.service';
import { InformationService } from '../services/information.service';

@Component({
    selector: 'app-filter',
    templateUrl: './filter.component.html',
    styleUrls: ['./filter.component.scss']
})
export class FilterComponent implements OnInit, AfterViewInit {


    @Input() inputDrawer!: MatDrawer;

    categories: Category[] = [];
    filteredCategories!: Observable<Category[]>;
    filteredDetails!: Observable<Source[]>;
    filterForm!: FormGroup;
    filters: string[] = [];
    details: Source[] = [];
    statusChips: any = [];
    years: any = { lower: 1968, upper: 2021 };
    searchCheckboxes: any = [];

    constructor(private fb: FormBuilder, private infoService: InformationService,
        private advancedSearchService: AdvancedSearchService,
        private imageFiltersService: ImageFiltersService) {
        infoService.Categories().subscribe(
            data => {
                this.categories = data;
            },
            error => { console.log(error) }
        );

        infoService.Source().subscribe(
            data => {
                this.details = data;
            },
            error => { console.log(error) }
        );

        infoService.Statuses().subscribe(
            data => {
                data.forEach(status => {
                    this.statusChips.push({ name: status, selected: true });
                    this.filters.push(`Status: ${status}`);
                    this.filterForm.patchValue({ 'statuses': [...this.statusChips] });
                });
            },
            error => { console.log(error) }
        );

        this.searchCheckboxes = [
            { name: "Title", checked: "true" },
            { name: "Image", checked: "true" },
            { name: "Content", checked: "true" },
            { name: "Category", checked: "true" },
            { name: "Details", checked: "true" }];
        this.searchCheckboxes.forEach((search: any) => this.filters.push(`Search in: ${search['name']}`));

    }

    ngOnInit(): void {
        this.filterForm = this.fb.group({
            category: [''],
            details: [''],
            statuses: [[...this.statusChips], Validators.required],
            searchFields: [[...this.searchCheckboxes], Validators.required]
        });

        this.filteredCategories = this.filterForm.controls['category'].valueChanges.pipe(
            startWith(''),
            map(value => typeof value === 'string' ? value : ''),
            map(name => name ? this._filter(this.categories, name) : this.categories.slice(0, 10))
        );

        this.filteredDetails = this.filterForm.controls['details'].valueChanges.pipe(
            startWith(''),
            map(value => typeof value === 'string' ? value : ''),
            map(name => name ? this._filter(this.details, name) : this.details.slice(0, 10))
        );

        this.imageFiltersService.imageSearchEvent.subscribe(
            () => this.TurnOnOnlyImageFilters()
        );
    }

    TurnOnOnlyImageFilters(): any {
        this.searchCheckboxes.forEach((checkbox: any) => {
            if (checkbox['name'] !== "Image")
                checkbox['checked'] = false;
        });

        this.filterForm.controls.searchFields.setValue(this.searchCheckboxes.filter((s: any) => s['checked']));
        let to_remove = this.searchCheckboxes.filter((s: any) => !s['checked']);
        to_remove.forEach((checkbox: any) => {
            this.RemoveFromFilters(`Search in: ${checkbox['name']}`);
        });
    }

    ngAfterViewInit(): void {
        this.advancedSearchService.Search(this.PrepareParams(), true);
    }
    get f() { return this.filterForm.controls; }

    CloseDrawer() {
        this.inputDrawer.close();
    }

    ClearCategory() {
        this.filterForm.patchValue({ category: '' });
    }

    ClearDetails() {
        this.filterForm.patchValue({ details: '' });
    }

    ToggleSelection(chip: MatChip) {
        chip.toggleSelected();
        var current: string[] = this.filterForm.get('statuses')!.value;
        var selected = this.statusChips.filter((c: any) => c.name === chip.value);
        selected[0]['selected'] = chip.selected;

        if (chip.selected) {
            current.push(chip.value);
            this.filters.push(`Status: ${chip.value}`);
        }
        else {
            var idx = current.indexOf(chip.value);
            current.splice(idx, 1);
            this.RemoveFromFilters(chip.value);
        }

        this.filterForm.controls.statuses.setValue(current);
    }

    Remove(filter: string) {
        this.RemoveFromFilters(filter);

        if (filter.indexOf("Status") >= 0) {
            var status = filter.split(": ")[1];
            var statusToChange = this.statusChips.filter((s: any) => s['name'] === status);
            statusToChange[0]['selected'] = false;

            // remove from form
            this.filterForm.controls.statuses.setValue(this.statusChips.filter((s: any) => s['selected']));
        }

        if (filter.indexOf("Search in") >= 0) {
            var search = filter.split(": ")[1];
            var checkboxToChange = this.searchCheckboxes.filter((s: any) => s['name'] === search);
            checkboxToChange[0]['checked'] = false;
            this.filterForm.controls.searchFields.setValue(this.searchCheckboxes.filter((s: any) => s['checked']));
        }
    }

    RemoveFromFilters(filter: string) {
        var fullValue = this.filters.filter(f => f.indexOf(filter) >= 0)[0];
        this.filters.splice(this.filters.indexOf(fullValue), 1);
    }

    DisplayCategory(cat: Category): string {
        return cat ? cat.name : "";
    }

    DisplayDetails(source: Source): string {
        return source ? source.name : "";
    }

    Filter() {
        this.advancedSearchService.Search(this.PrepareParams());
        this.inputDrawer.close();
    }

    PrepareParams(): QueryParams {
        var params: QueryParams = {
            status: this.statusChips.filter((s: any) => s.selected).map((s: any) => s.name),
            category: this.filters.filter(f => f.includes("Category:")).map(f => f.split(": ")[1]),
            details: this.filters.filter(f => f.includes("Details:")).map(f => f.split(": ")[1]),
            yearFrom: this.years.lower,
            yearTo: this.years.upper,
            fields: this.searchCheckboxes.filter((s: any) => s.checked).map((s: any) => s.name),
            sort: null,
            sortAsc: false,
        };

        return params;
    }

    private _filter(list: any, name: any): any {
        const filterValue = name.toLowerCase();

        return list.filter((item: any) => item.name.toLowerCase().indexOf(filterValue) > -1);
    }

    SelectedCategory(event: any) {
        var name = `Category: ${event.option.value.name}`;
        if (this.filters.indexOf(name) < 0)
        {
            this.filters.push(name);
            this.ClearCategory();
        }
    }

    SelectedDetails(event: any) {
        var name = `Details: ${event.option.value.name}`;
        if (this.filters.indexOf(name) < 0)
        {
            this.filters.push(name);
            this.ClearDetails();
        }
    }

    CheckedSearch(event: any, name: string) {
        var checked = this.searchCheckboxes.filter((c: any) => c.name === name);
        checked[0]['checked'] = event.checked;
        var nameToSearch = `Search in: ${name}`;

        if (event.checked) {
            this.filters.push(nameToSearch);
            this.filterForm.controls.searchFields.setValue(this.searchCheckboxes.filter((s: any) => s['checked']));
        } else {
            this.Remove(nameToSearch);
        }
    }
}

