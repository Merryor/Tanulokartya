import { Deck } from '../models/deck';
import { User } from './user';

/**
 * DeckAssignment interface
 */
export interface DeckAssignment {
    deckId: number;
    deck: Deck;
    userId: string;
    user: User;
}
