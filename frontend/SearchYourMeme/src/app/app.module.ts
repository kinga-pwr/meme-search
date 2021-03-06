import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { BrowserModule } from '@angular/platform-browser';
import { FilterComponent } from './filter/filter.component';
import { FlexLayoutModule } from '@angular/flex-layout';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { InfiniteScrollModule } from 'ngx-infinite-scroll'
import { IonicModule } from '@ionic/angular';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatChipsModule } from '@angular/material/chips';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MemeListComponent } from './meme-list/meme-list.component';
import { MemeListItemComponent } from './meme-list-item/meme-list-item.component';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { SearchComponent } from './search/search.component';
import { TextSearchComponent } from './text-search/text-search.component';
import { ImageSearchDialogComponent } from './image-search-dialog/image-search-dialog.component';
import {MatDialogModule} from '@angular/material/dialog';
import { MemeDetailsDialogComponent } from './meme-details-dialog/meme-details-dialog.component';
import {MatSnackBarModule} from '@angular/material/snack-bar';
import { ErrorDialogComponent } from './error-dialog/error-dialog.component';

@NgModule({
    declarations: [
        AppComponent,
        SearchComponent,
        MemeListItemComponent,
        MemeListComponent,
        TextSearchComponent,
        FilterComponent,
        ImageSearchDialogComponent,
        MemeDetailsDialogComponent,
        ErrorDialogComponent
    ],
    imports: [
        AppRoutingModule,
        BrowserAnimationsModule,
        BrowserModule,
        FlexLayoutModule,
        FormsModule,
        HttpClientModule,
        InfiniteScrollModule,
        MatAutocompleteModule,
        MatButtonModule,
        MatCardModule,
        MatCheckboxModule,
        MatChipsModule,
        MatFormFieldModule,
        MatGridListModule,
        MatIconModule,
        MatInputModule,
        MatProgressBarModule,
        MatProgressSpinnerModule,
        MatSidenavModule,
        MatTooltipModule,
        ReactiveFormsModule,
        MatDialogModule,
        MatSnackBarModule,
        IonicModule.forRoot(),
    ],
    providers: [],
    bootstrap: [AppComponent]
})
export class AppModule { }
