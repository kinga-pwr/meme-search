import { stringify } from '@angular/compiler/src/util';
import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatChip, MatChipList } from '@angular/material/chips';
import { MatDrawer } from '@angular/material/sidenav';
import { Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';
import { InformationService } from '../services/information.service';

@Component({
    selector: 'app-filter',
    templateUrl: './filter.component.html',
    styleUrls: ['./filter.component.scss']
})
export class FilterComponent implements OnInit {


    @Input() inputDrawer!: MatDrawer;
    @ViewChild('chipList') chipList!: MatChipList;
    categories: string[] = [];
    filteredCategories!: Observable<string[]>;
    filteredSources!: Observable<string[]>;
    filterForm!: FormGroup;
    filters = ["Status: Confirmed", "Status: Submitted", "Status: Deadpool"];
    sources: string[] = [];
    statusChips = ["Confirmed", "Submitted", "Deadpool"];

    constructor(private fb: FormBuilder, private infoService: InformationService) {
        infoService.Categories().subscribe(
            data => {
                for (var cat in data) {
                    this.categories.push(cat);
                }
            },
            error => { console.log("error") }
        );

        infoService.Source().subscribe(
            data => {
                for (var source in data) {
                    this.sources.push(source);
                }
            },
            error => { console.log("error") }
        );
    }

    ngOnInit(): void {
        this.filterForm = this.fb.group({
            category: [''],
            source: [''],
            statuses: [[...this.statusChips], Validators.required],
            minYear: [1968],
            maxYear: [2021]
        });

        this.filteredCategories = this.filterForm.controls['category'].valueChanges.pipe(
            startWith(''),
            map(value => typeof value === 'string' ? value : ''),
            map(name => name ? this._filter(name) : this.categories.slice(0, 10))
        );

        this.filteredSources = this.filterForm.controls['source'].valueChanges.pipe(
            startWith(''),
            map(value => typeof value === 'string' ? value : ''),
            map(name => name ? this._filter(name) : this.sources.slice(0, 10))
        );
    }

    get f() { return this.filterForm.controls; }

    CloseDrawer() {
        this.inputDrawer.close();
    }

    ClearCategory() {
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

    private _filter(name: any): any {
        const filterValue = name.toLowerCase();

        return this.categories.filter(cat => cat.toLowerCase().indexOf(filterValue) > -1);
    }
}

