import { HttpErrorResponse } from '@angular/common/http';
import { getErrorMessage } from './api-error';

describe('getErrorMessage', () => {
  it('validation hatasında genel "dto required" yerine asıl alan hatasını gösterir', () => {
    const error = new HttpErrorResponse({
      status: 400,
      error: {
        title: 'One or more validation errors occurred.',
        errors: {
          dto: ['The dto field is required.'],
          '$.venueId': ['Geçersiz id: uydurma']
        }
      }
    });

    expect(getErrorMessage(error)).toBe('Geçersiz id: uydurma');
  });

  it('alan hatası yoksa ilk validation mesajını gösterir', () => {
    const error = new HttpErrorResponse({
      status: 400,
      error: { errors: { Title: ['Başlık zorunludur.'] } }
    });

    expect(getErrorMessage(error)).toBe('Başlık zorunludur.');
  });
});
